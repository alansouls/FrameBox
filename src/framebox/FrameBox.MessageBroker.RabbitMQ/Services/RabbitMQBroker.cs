using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace FrameBox.MessageBroker.RabbitMQ.Services;

public class RabbitMQBroker : IMessageBroker
{
    private readonly IConnection _connection;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQBroker> _logger;
    private IChannel? _channel;
    private readonly IServiceProvider _serviceProvider;
    private readonly MessageHeaderHolder _messageHeaderHolder;

    public RabbitMQBroker(IOptions<RabbitMQOptions> options, ILogger<RabbitMQBroker> logger, IServiceProvider serviceProvider, MessageHeaderHolder messageHeaderHolder)
    {
        _options = options.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _connection = string.IsNullOrEmpty(_options.ConnectionKey) ?
            serviceProvider.GetRequiredService<IConnection>() :
            serviceProvider.GetRequiredKeyedService<IConnection>(_options.ConnectionKey);
        _messageHeaderHolder = messageHeaderHolder;
    }

    public async Task SendMessagesAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken)
        where T : class, IMessage
    {
        var exchangeName = _options.GetExchangeName<T>();
        try
        {
            _channel ??= await CreateChannel(exchangeName, cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ broker. Please check your connection settings.");

            throw new FailedToSendMessagesException<T>(messages, ex);
        }

        var failedMessages = (await Task.WhenAll(messages.Select(async message =>
        {
            var messageBody = message.ToJson();

            var routingKey = CreateRoutingKey(message);

            try
            {
                var headers = _messageHeaderHolder.GetHeaders(message.EventId);

                var basicProperties = new BasicProperties
                {
                    Headers = headers.ToDictionary<KeyValuePair<string, string>, string, object?>(s => s.Key, s => s.Value),
                };

                await _channel.BasicPublishAsync(exchangeName,
                    routingKey,
                    mandatory: false,
                    basicProperties,
                    messageBody,
                    cancellationToken);
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

    /// <summary>
    /// Outbox messages are published to a topic exchange keyed by the event name, so listeners can bind
    /// only to the events they handle. Inbox messages go through the default exchange, where the routing
    /// key must be the queue name.
    /// </summary>
    private string CreateRoutingKey<T>(T message) where T : class, IMessage => message switch
    {
        OutboxMessage outboxMessage => outboxMessage.EventType,
        InboxMessage => _options.InboxQueueName,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    private async Task<IChannel> CreateChannel(string exchangeName, CancellationToken cancellationToken)
    {
        var channel = await _connection.CreateChannelAsync(null, cancellationToken); //TODO: check options

        if (!string.IsNullOrWhiteSpace(exchangeName))
        {
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true,
                cancellationToken: cancellationToken);
        }

        return channel;
    }
}