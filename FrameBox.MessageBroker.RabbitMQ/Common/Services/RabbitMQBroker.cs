using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text.Json;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Services;

public class RabbitMQBroker : IMessageBroker
{
    private readonly IConnection _connection;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQBroker> _logger;
    private IChannel? _channel;

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
            _channel ??= await CreateChannel(cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ broker. Please check your connection settings.");

            throw new FailedToSendMessagesException<T>(messages, ex);
        }

        var failedMessages = (await Task.WhenAll(messages.Select(async message =>
        {
            string exchangeName = message switch
            {
                InboxMessage => _options.InboxExchangeName,
                OutboxMessage => _options.OutboxExchangeName,
                _ => throw new InvalidOperationException("Unsupported message type.")
            };

            var messageBody = message.ToJson();

            try
            {
                await _channel.BasicPublishAsync(exchangeName, string.Empty, messageBody, cancellationToken);
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

    private async Task<IChannel> CreateChannel(CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync(null, cancellationToken); //TODO: check options
        await channel.ExchangeDeclareAsync(_options.OutboxExchangeName, ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: cancellationToken);
        await channel.ExchangeDeclareAsync(_options.InboxExchangeName, ExchangeType.Direct, durable: true, autoDelete: false, cancellationToken: cancellationToken);

        return channel;
    }
}
