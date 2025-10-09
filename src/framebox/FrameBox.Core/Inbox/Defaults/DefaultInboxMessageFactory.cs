using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Defaults;

public class DefaultInboxMessageFactory : IInboxMessageFactory
{
    private readonly IEventHandlerProvider _eventHandlerProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IEventRegistry _eventRegistry;

    public DefaultInboxMessageFactory(IEventHandlerProvider eventHandlerProvider, TimeProvider timeProvider, IEventRegistry eventRegistry)
    {
        _eventHandlerProvider = eventHandlerProvider;
        _timeProvider = timeProvider;
        _eventRegistry = eventRegistry;
    }

    public IEnumerable<InboxMessage> CreateMessages(IEvent @event)
    {
        var handlers = _eventHandlerProvider.GetEventHandlerTypes(@event.GetType());

        return handlers.Select(handlerType => new InboxMessage(
            Guid.NewGuid(),
            @event.Id,
            _eventRegistry.GetHandlerName(handlerType),
            _timeProvider.GetUtcNow()));
    }
}
