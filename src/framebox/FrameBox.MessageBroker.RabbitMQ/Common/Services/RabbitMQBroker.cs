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
            _channel ??= await CreateChannel<T>(cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ broker. Please check your connection settings.");

            throw new FailedToSendMessagesException<T>(messages, ex);
        }

        var failedMessages = (await Task.WhenAll(messages.Select(async message =>
        {
            var messageBody = message.ToJson();

            try
            {
                await _channel.BasicPublishAsync(exchange: string.Empty, routingKey: _options.GetQueueName<T>(), messageBody, cancellationToken);
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

    private async Task<IChannel> CreateChannel<T>(CancellationToken cancellationToken) where T : class, IMessage
    {
        var channel = await _connection.CreateChannelAsync(null, cancellationToken); //TODO: check options
        await channel.QueueDeclareAsync(_options.GetQueueName<T>(), durable: true, autoDelete: false, cancellationToken: cancellationToken);
        return channel;
    }
}
