namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextRestorer
{
    Task RestoreAsync(IEventContext context, CancellationToken cancellationToken);
}
