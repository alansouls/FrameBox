using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxHandler : IMessageHandler<InboxMessage>
{
}
