using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextManager
{
    Task CaptureContextsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken);
}
