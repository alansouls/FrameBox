using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.MessageBroker.RabbitMQ.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text.Json;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Services;

public class RabbitMQBroker : IMessageBroker
{
    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly IConnection _connection;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQBroker> _logger;
    private IChannel? _outboxChannel;

    public RabbitMQBroker(IConnection connection, IOptions<RabbitMQOptions> options, ILogger<RabbitMQBroker> logger)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendMessagesAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken) where T : class, IMessage
    {
        try
        {
            _outboxChannel ??= await CreateChannel(_options.OutboxExchangeName, cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ broker. Please check your connection settings.");

            throw new FailedToSendMessagesException<T>(messages, ex);
        }

        var failedMessages = (await Task.WhenAll(messages.Select(async message =>
        {
            var messageBody = JsonSerializer.SerializeToUtf8Bytes(message, _serializerOptions);
            try
            {
                await _outboxChannel.BasicPublishAsync(_options.OutboxExchangeName, string.Empty, messageBody, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message {MessageId}", message.Id);
                return message;
            }

            return null;
        }))).Where(message => message is not null).ToList();

        if (failedMessages.Count > 0)
        {
            throw new FailedToSendMessagesException<T>(failedMessages!);
        }
    }

    private async Task<IChannel> CreateChannel(string exchangeName, CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync(null, cancellationToken); //TODO: check options
        await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);

        return channel;
    }
}
