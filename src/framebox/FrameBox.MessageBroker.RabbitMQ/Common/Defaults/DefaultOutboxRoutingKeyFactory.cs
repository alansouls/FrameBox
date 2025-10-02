using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Common.Interfaces;
using FrameBox.MessageBroker.RabbitMQ.Common.Options;
using Microsoft.Extensions.Options;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Defaults;

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