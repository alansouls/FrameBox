using System.Reflection;
using System.Runtime.Loader;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Inbox.Defaults;

public class DefaultInboxMessageFactory : IInboxMessageFactory
{
    private readonly IEventHandlerProvider _eventHandlerProvider;
    private readonly IDomainEventSerializer _domainEventSerializer;

    public DefaultInboxMessageFactory(IEventHandlerProvider eventHandlerProvider,
        IDomainEventSerializer domainEventSerializer)
    {
        _eventHandlerProvider = eventHandlerProvider;
        _domainEventSerializer = domainEventSerializer;
    }

    public async ValueTask<IEnumerable<InboxMessage>> CreateFromOutboxMessage(OutboxMessage message,
        CancellationToken cancellationToken)
    {
        var domainEventType = Type.GetType(message.Type);

        if (domainEventType == null)
        {
            throw new InvalidOperationException($"Type '{message.Type}' could not be found.");
        }

        if (!typeof(IDomainEvent).IsAssignableFrom(domainEventType))
        {
            throw new InvalidOperationException($"Type '{message.Type}' does not implement IDomainEvent.");
        }

        var domainEventResult = await _domainEventSerializer.DeserializeAsync(message.Payload, domainEventType, cancellationToken);

        if (!domainEventResult.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to deserialize domain event: {domainEventResult.Errors.First()}");
        }
        
        var domainEvent = domainEventResult.Value;
        
        var handlers = _eventHandlerProvider.GetHandlers(domainEvent);
    }
}