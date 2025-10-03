using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Outbox.Interfaces;

namespace FrameBox.Core.Events.Defaults;

internal sealed class DefaultEventDispatcher : IEventDispatcher
{
    private readonly IOutboxStorage _outboxStorage;
    private readonly IOutboxMessageFactory _outboxMessageFactory;
    private readonly IMessageBroker _messageBroker;
    private readonly TimeProvider _timeProvider;

    public DefaultEventDispatcher(IOutboxStorage outboxStorage, IOutboxMessageFactory outboxMessageFactory, IMessageBroker messageBroker, TimeProvider timeProvider)
    {
        _outboxStorage = outboxStorage;
        _outboxMessageFactory = outboxMessageFactory;
        _messageBroker = messageBroker;
        _timeProvider = timeProvider;
    }

    public async Task DispatchAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
    {
        var sendingDate = _timeProvider.GetUtcNow();

        var outboxMessages = await Task.WhenAll(events.Select(async e =>
        {
            var message = await _outboxMessageFactory.CreateAsync(e, cancellationToken);
            message.SetAsSending(sendingDate);
            return message;
        }));

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
