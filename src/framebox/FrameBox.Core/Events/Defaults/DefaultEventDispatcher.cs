using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Outbox.Interfaces;

namespace FrameBox.Core.Events.Defaults;

internal sealed class DefaultEventDispatcher : IEventDispatcher
{
    private readonly IOutboxStorage _outboxStorage;
    private readonly IOutboxMessageFactory _outboxMessageFactory;
    private readonly IMessageBroker _messageBroker;
    private readonly TimeProvider _timeProvider;
    private readonly IEventContextManager _eventContextManager;

    public DefaultEventDispatcher(IOutboxStorage outboxStorage, IOutboxMessageFactory outboxMessageFactory, IMessageBroker messageBroker, IEventContextManager eventContextManager, TimeProvider timeProvider)
    {
        _outboxStorage = outboxStorage;
        _outboxMessageFactory = outboxMessageFactory;
        _messageBroker = messageBroker;
        _timeProvider = timeProvider;
        _eventContextManager = eventContextManager;
    }

    public async Task DispatchAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
    {
        var eventList = events.ToList();
        var sendingDate = _timeProvider.GetUtcNow();

        var outboxMessages = await Task.WhenAll(eventList.Select(async e =>
        {
            var message = await _outboxMessageFactory.CreateAsync(e, cancellationToken);
            message.SetAsSending(sendingDate);
            return message;
        }));

        await _eventContextManager.CaptureContextsAsync(eventList, cancellationToken);

        await _outboxStorage.AddMessagesAsync(outboxMessages, cancellationToken);

        await _messageBroker.SendMessagesAsync(outboxMessages, cancellationToken);

        var sentDate = _timeProvider.GetUtcNow();

        foreach (var message in outboxMessages)
        {
            message.SetAsSent(sentDate);
        }

        await _outboxStorage.UpdateMessagesAsync(outboxMessages, cancellationToken);
    }
}
