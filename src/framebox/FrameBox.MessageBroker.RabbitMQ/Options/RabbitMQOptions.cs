using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.MessageBroker.RabbitMQ.Options;

public class RabbitMQOptions
{
    public string? ConnectionKey { get; set; }
    public string OutboxExchangeName { get; set; } = "outbox_topic";
    
    public string OutboxQueueName { get; set; } = "outbox_queue";
    
    public string OutboxTopicName { get; set; } = "outbox_queue.#";

    public int OutboxMaxConcurrency { get; set; } = 5000;

    public string InboxQueueName { get; set; } = "inbox_queue";

    public int InboxMaxConcurrency { get; set; } = 1000;
    
    public string GetExchangeName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(OutboxMessage) => OutboxExchangeName,
        var t when t == typeof(InboxMessage) => string.Empty,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };
    
    public string GetTopicName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(OutboxMessage) => OutboxTopicName,
        var t when t == typeof(InboxMessage) => string.Empty,
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
