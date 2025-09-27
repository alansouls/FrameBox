using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Options;

public class RabbitMQOptions
{
    public string OutboxExchangeName { get; set; } = "outbox_exchange";

    public string OutboxQueueName { get; set; } = "outbox_queue";

    public int OutboxMaxConcurrency { get; set; } = 5000;

    public string InboxExchangeName { get; set; } = "inbox_exchange";

    public string InboxQueueName { get; set; } = "inbox_queue";

    public int InboxMaxConcurrency { get; set; } = 1000;

    public string GetExchangeName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(InboxMessage) => InboxExchangeName,
        var t when t == typeof(OutboxMessage) => OutboxExchangeName,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    public string GetQueueName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(InboxMessage) => InboxQueueName,
        var t when t == typeof(OutboxMessage) => OutboxQueueName,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    public int GetMaxConcurrency<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(InboxMessage) => InboxMaxConcurrency,
        var t when t == typeof(OutboxMessage) => OutboxMaxConcurrency,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };
}
