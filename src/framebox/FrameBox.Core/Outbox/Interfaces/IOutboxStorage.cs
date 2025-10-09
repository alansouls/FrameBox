using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Outbox.Interfaces;

public interface IOutboxStorage
{
    Task AddMessagesAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default);

    Task<IEnumerable<OutboxMessage>> GetMessagesToSendAsync(int maxCount, CancellationToken cancellationToken = default);

    Task UpdateMessagesAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default);

    Task<OutboxMessage?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task<IEnumerable<OutboxMessage>> GetMessagesToTimeoutAsync(int maxCount, CancellationToken cancellationToken = default);
}
