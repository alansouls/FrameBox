using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using FrameBox.MessageBroker.RabbitMQ.Options;
using FrameBox.MessageBroker.RabbitMQ.Interfaces;

namespace FrameBox.MessageBroker.RabbitMQ.Services;

public class RabbitMQBroker : IMessageBroker
{
    private readonly IConnection _connection;
    private readonly RabbitMQOptions _options;
    private readonly ILogger<RabbitMQBroker> _logger;
    private IChannel? _channel;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQBroker(IConnection connection, IOptions<RabbitMQOptions> options, ILogger<RabbitMQBroker> logger, IServiceProvider serviceProvider)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
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
        
        var routingKeyFactory = _serviceProvider.GetRequiredService<IRoutingKeyFactory<T>>();

        var failedMessages = (await Task.WhenAll(messages.Select(async message =>
        {
            var messageBody = message.ToJson();
            
            var routingKey = routingKeyFactory.CreateRoutingKey(message);

            try
            {
                await _channel.BasicPublishAsync(exchange: exchangeName, routingKey,
                    messageBody, cancellationToken);
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

        if (!string.IsNullOrWhiteSpace(exchangeName))
        {
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true,
                cancellationToken: cancellationToken);
        }

        return channel;
    }
}