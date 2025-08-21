using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Events.Defaults;

public abstract class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IDomainEvent
{
    public Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        if (@event is TEvent typedEvent)
        {
            return HandleAsync(typedEvent, cancellationToken);
        }

        throw new ArgumentException($"Event type mismatch: expected {typeof(TEvent).Name}, but received {@event.GetType().Name}");
    }

    public abstract Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}