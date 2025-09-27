using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Outbox.Interfaces;

public interface IOutboxHandler : IMessageHandler<OutboxMessage>
{
}
