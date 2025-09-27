namespace FrameBox.Core.Events.Interfaces;

public interface IEventHandler
{
    Task HandleAsync(IDomainEvent @event, CancellationToken cancellationToken);
}

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
