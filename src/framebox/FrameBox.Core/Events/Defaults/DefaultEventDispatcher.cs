using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Outbox.Interfaces;

namespace FrameBox.Core.Events.Defaults;

internal sealed class DefaultEventDispatcher : IEventDispatcher
{
    private readonly IOutboxStorage _outboxStorage;
    private readonly IOutboxMessageFactory _outboxMessageFactory;
    private readonly IMessageBroker _messageBroker;

    public DefaultEventDispatcher(IOutboxStorage outboxStorage, IOutboxMessageFactory outboxMessageFactory, IMessageBroker messageBroker)
    {
        _outboxStorage = outboxStorage;
        _outboxMessageFactory = outboxMessageFactory;
        _messageBroker = messageBroker;
    }

    public async Task DispatchAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
    {
        var outboxMessages = await Task.WhenAll(events.Select(e => _outboxMessageFactory.CreateAsync(e, cancellationToken).AsTask()));

        await _outboxStorage.AddMessagesAsync(outboxMessages, cancellationToken);

        await _messageBroker.SendMessagesAsync(outboxMessages, cancellationToken);
    }
}
