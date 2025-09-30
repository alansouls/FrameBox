namespace FrameBox.Core.Events.Interfaces;

public interface IEventHandler
{
    Task HandleAsync(IEvent @event, CancellationToken cancellationToken);
}

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
