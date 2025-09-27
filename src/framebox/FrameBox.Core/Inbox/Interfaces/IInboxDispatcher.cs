using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxDispatcher
{
    void RunNow();
}
