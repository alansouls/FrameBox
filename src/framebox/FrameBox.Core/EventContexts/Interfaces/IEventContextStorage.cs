namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextStorage
{
    Task AddAsync(IEnumerable<IEventContext> eventContexts, CancellationToken cancellationToken);

    Task<IEnumerable<IEventContext>> GetEventContextsAsync(Guid eventId, CancellationToken cancellationToken);
}
