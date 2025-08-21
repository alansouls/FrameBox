using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxProcessor
{
    Task ProcessInboxMessageAsync(InboxMessage message, CancellationToken cancellationToken);
}