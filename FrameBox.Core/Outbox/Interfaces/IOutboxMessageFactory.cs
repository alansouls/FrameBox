using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Outbox.Interfaces;

public interface IOutboxMessageFactory
{
    ValueTask<OutboxMessage> CreateAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
}
