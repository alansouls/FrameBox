using FrameBox.Core.Inbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Interfaces;
using FrameBox.MessageBroker.RabbitMQ.Options;
using Microsoft.Extensions.Options;

namespace FrameBox.MessageBroker.RabbitMQ.Defaults;

public class DefaultInboxRoutingKeyFactory : IRoutingKeyFactory<InboxMessage>
{
    private readonly RabbitMQOptions _options;

    public DefaultInboxRoutingKeyFactory(IOptions<RabbitMQOptions> options)
    {
        _options = options.Value;
    }

    public string CreateRoutingKey(InboxMessage message)
    {
        return _options.InboxQueueName;
    }
}