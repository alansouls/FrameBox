using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.MessageBroker.AzureServiceBus.Options;

public class AzureServiceBusOptions
{
    public string? ConnectionKey { get; set; }
    public string OutboxTopicName { get; set; } = "framebox-outbox";

    public string OutboxSubscriptionName { get; set; } = "framebox-outbox-subscription";

    public string InboxQueueName { get; set; } = "framebox-inbox-queue";

    public int OutboxMaxConcurrency { get; set; } = 5000;

    public int InboxMaxConcurrency { get; set; } = 1000;

    public bool IsTopicSubscription<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(OutboxMessage) => true,
        var t when t == typeof(InboxMessage) => false,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    public string GetTopicName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(OutboxMessage) => OutboxTopicName,
        var t when t == typeof(InboxMessage) => string.Empty,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    public string GetSubscriptionName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(OutboxMessage) => OutboxSubscriptionName,
        var t when t == typeof(InboxMessage) => string.Empty,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    public string GetQueueName<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(OutboxMessage) => string.Empty,
        var t when t == typeof(InboxMessage) => InboxQueueName,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };

    public int GetMaxConcurrency<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(InboxMessage) => InboxMaxConcurrency,
        var t when t == typeof(OutboxMessage) => OutboxMaxConcurrency,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };
}
