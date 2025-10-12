using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Defaults;

public class DefaultInboxMessageFactory : IInboxMessageFactory
{
    private readonly IDomainEventSerializer _domainEventSerializer;
    private readonly IEventHandlerProvider _eventHandlerProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IEventRegistry _eventRegistry;

    public DefaultInboxMessageFactory(IEventHandlerProvider eventHandlerProvider, TimeProvider timeProvider, IEventRegistry eventRegistry, IDomainEventSerializer domainEventSerializer)
    {
        _eventHandlerProvider = eventHandlerProvider;
        _timeProvider = timeProvider;
        _eventRegistry = eventRegistry;
        _domainEventSerializer = domainEventSerializer;
    }

    public async ValueTask<IEnumerable<InboxMessage>> CreateMessages(IEvent @event, CancellationToken cancellationToken)
    {
        var eventPayload = await _domainEventSerializer.SerializeAsync(@event, cancellationToken);
        
        var handlers = _eventHandlerProvider.GetEventHandlerTypes(@event.GetType());

        return handlers.Select(handlerType => new InboxMessage(
            Guid.NewGuid(),
            @event.Id,
            _eventRegistry.GetEventName(@event.GetType()),
            eventPayload,
            _eventRegistry.GetHandlerName(handlerType),
            _timeProvider.GetUtcNow()));
    }
}
