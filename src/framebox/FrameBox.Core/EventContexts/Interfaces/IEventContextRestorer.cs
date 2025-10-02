namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextRestorer
{
    Task RestoreAsync(Guid eventId, CancellationToken cancellationToken);
}
