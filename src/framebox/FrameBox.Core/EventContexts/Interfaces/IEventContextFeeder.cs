using FrameBox.Core.EventContexts.Models;

namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextFeeder
{
    string Type { get; }
    Task FeedAsync(IEventContext context, CancellationToken cancellationToken);
}
