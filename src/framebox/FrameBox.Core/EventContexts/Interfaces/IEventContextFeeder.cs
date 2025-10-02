using FrameBox.Core.EventContexts.Models;

namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextFeeder
{
    Task FeedAsync(IEventContext context, CancellationToken cancellationToken);
}
