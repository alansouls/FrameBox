namespace FrameBox.Core.Events.Interfaces;

public interface IEventDispatcher
{
    Task DispatchAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken);
}
