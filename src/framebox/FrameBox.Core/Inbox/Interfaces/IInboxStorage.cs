using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxStorage
{
    Task AddMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default);

    Task<IEnumerable<InboxMessage>> GetMessagesReadyToSendAsync(int maxCount, CancellationToken cancellationToken = default);

    Task UpdateMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default);

    Task<bool> ExistsInboxByOutboxIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default);
}
