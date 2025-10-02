using System.Collections;
using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextManager
{
    Task<IEnumerable<IEventContext>> CaptureContextsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken);

    Task RestoreContextAsync(IEnumerable<IEventContext> eventContexts, CancellationToken cancellationToken);
}
