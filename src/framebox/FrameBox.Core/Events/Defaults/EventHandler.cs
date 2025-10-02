using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Events.Defaults;

public abstract class EventHandler<TDomainEvent> : IEventHandler<TDomainEvent> where TDomainEvent : IEvent
{
    public abstract Task HandleAsync(TDomainEvent @event, CancellationToken cancellationToken);

    public async Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
    {
        await HandleAsync((TDomainEvent)@event ?? throw new InvalidOperationException("Invalid domain event for this handler"), cancellationToken);
    }
}
