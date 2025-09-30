using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Events.Defaults;

public abstract class EventHandler<TDomainEvent> : IEventHandler<TDomainEvent> where TDomainEvent : IEvent
{
    public abstract Task HandleAsync(TDomainEvent @event, CancellationToken cancellationToken);

    public Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
         => HandleAsync((TDomainEvent)@event ?? throw new InvalidOperationException("Invalid domain event for this handler"), cancellationToken);
}
