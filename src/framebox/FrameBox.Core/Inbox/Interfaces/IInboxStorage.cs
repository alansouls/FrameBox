using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Inbox.Interfaces;

public interface IInboxStorage
{
    Task AddMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default);

    Task<IEnumerable<InboxMessage>> GetMessagesReadyToSendAsync(int maxCount, CancellationToken cancellationToken = default);

    Task UpdateMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default);

    Task<bool> ExistsInboxByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

    Task<IEnumerable<InboxMessage>> GetMessagesToTimeoutAsync(int maxCount, CancellationToken cancellationToken = default);

    Task<IEnumerable<InboxMessage>> GetMessagesToCleanupAsync(int maxCount, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default);

    Task<int> DeleteMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default);
}
