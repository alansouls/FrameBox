using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxMessageFactory
{
    ValueTask<IEnumerable<InboxMessage>> CreateFromOutboxMessage(OutboxMessage message,
        CancellationToken cancellationToken);
}