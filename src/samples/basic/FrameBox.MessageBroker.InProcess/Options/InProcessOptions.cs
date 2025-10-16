using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.MessageBroker.AzureServiceBus.Options;

public class InProcessOptions
{
    public int OutboxMaxConcurrency { get; set; } = 5000;

    public int InboxMaxConcurrency { get; set; } = 1000;

    public int GetMaxConcurrency<TMessage>() where TMessage : class, IMessage => typeof(TMessage) switch
    {
        var t when t == typeof(InboxMessage) => InboxMaxConcurrency,
        var t when t == typeof(OutboxMessage) => OutboxMaxConcurrency,
        _ => throw new InvalidOperationException("Unsupported message type.")
    };
}
