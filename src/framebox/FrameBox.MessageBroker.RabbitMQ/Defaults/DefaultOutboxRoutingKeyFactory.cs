using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Interfaces;
using FrameBox.MessageBroker.RabbitMQ.Options;
using Microsoft.Extensions.Options;

namespace FrameBox.MessageBroker.RabbitMQ.Defaults;

public class DefaultOutboxRoutingKeyFactory : IRoutingKeyFactory<OutboxMessage>
{
    private readonly RabbitMQOptions _options;

    public DefaultOutboxRoutingKeyFactory(IOptions<RabbitMQOptions> options)
    {
        _options = options.Value;
    }

    public string CreateRoutingKey(OutboxMessage message)
    {
        return _options.OutboxQueueName + "." + message.Type;
    }
}