using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Defaults;

public class DefaultInboxMessageFactory : IInboxMessageFactory
{
    private readonly IEventHandlerProvider _eventHandlerProvider;
    private readonly TimeProvider _timeProvider;

    public DefaultInboxMessageFactory(IEventHandlerProvider eventHandlerProvider, TimeProvider timeProvider)
    {
        _eventHandlerProvider = eventHandlerProvider;
        _timeProvider = timeProvider;
    }

    public IEnumerable<InboxMessage> CreateMessages(IEvent @event)
    {
        var handlers = _eventHandlerProvider.GetEventHandlerTypes(@event.GetType());

        return handlers.Select(handlerType => new InboxMessage(
            Guid.NewGuid(),
            @event.Id,
            handlerType.AssemblyQualifiedName!,
            _timeProvider.GetUtcNow()));
    }
}
