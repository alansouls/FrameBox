using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxMessageFactory
{
    IEnumerable<InboxMessage> CreateMessages(IEvent @event);
}
