using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Storage.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FrameBox.Storage.EFCore.Outbox;

internal class InboxDbContextStorage : IInboxStorage
{
    private readonly InternalDbContextWrapper<IInboxStorage> _dbContextWrapper;
    private readonly TimeProvider _timeProvider;

    public InboxDbContextStorage(InternalDbContextWrapper<IInboxStorage> dbContextWrapper, TimeProvider timeProvider)
    {
        _dbContextWrapper = dbContextWrapper;
        _timeProvider = timeProvider;
    }

    public async Task AddMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default)
    {
        await _dbContextWrapper.Context.AddRangeAsync(messages, cancellationToken);

        await _dbContextWrapper.Context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default)
    {
        _dbContextWrapper.Context.UpdateRange(messages);
        await _dbContextWrapper.Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<InboxMessage>> GetMessagesReadyToSendAsync(int maxCount, CancellationToken cancellationToken)
    {
        if (maxCount <= 0)
        {
            return [];
        }

        var executionStrategy = _dbContextWrapper.Context.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteInTransactionAsync(async (ct) =>
        {
            var processId = Guid.NewGuid();

            var timeStamp = _timeProvider.GetUtcNow();

            var messagesToUpdateQuery = _dbContextWrapper.Context
                .Set<InboxMessage>()
                .Where(m => m.State == Core.Inbox.Enums.InboxState.ReadyToRetry)
                .OrderBy(m => m.UpdatedAt)
                .Take(maxCount);

            await messagesToUpdateQuery.ExecuteUpdateAsync(m => m.SetProperty(x => x.State, Core.Inbox.Enums.InboxState.Retrying)
                .SetProperty(x => x.UpdatedAt, timeStamp).SetProperty(x => x.ProcessId, processId), cancellationToken);

            var messages = await _dbContextWrapper.Context
                .Set<InboxMessage>()
                .Where(m => m.ProcessId == processId)
                .ToListAsync(cancellationToken);

            return messages;
        }, verifySucceeded: (ct) => _dbContextWrapper.Context.Set<InboxMessage>().AnyAsync(o => o.ProcessId == o.ProcessId, cancellationToken: ct), isolationLevel: System.Data.IsolationLevel.RepeatableRead, cancellationToken);
    }

    public Task<bool> ExistsInboxByOutboxIdAsync(Guid outboxMessageId, CancellationToken cancellationToken = default)
    {
        return _dbContextWrapper.Context.Set<InboxMessage>().AnyAsync(m => m.OutboxMessageId == outboxMessageId, cancellationToken);
    }

    public async Task<IEnumerable<InboxMessage>> GetMessagesToTimeoutAsync(int maxCount, CancellationToken cancellationToken = default)
    {
        if (maxCount <= 0)
        {
            return [];
        }

        return await _dbContextWrapper.Context.Set<InboxMessage>()
            .Where(m => m.State == Core.Inbox.Enums.InboxState.Running 
                    || m.State == Core.Inbox.Enums.InboxState.Retrying 
                    || m.State == Core.Inbox.Enums.InboxState.Pending)
            .OrderBy(m => m.UpdatedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InboxMessage>> GetMessagesToCleanupAsync(int maxCount, DateTimeOffset cutoffDate, CancellationToken cancellationToken = default)
    {
        if (maxCount <= 0)
        {
            return [];
        }

        return await _dbContextWrapper.Context.Set<InboxMessage>()
            .Where(m => (m.State == Core.Inbox.Enums.InboxState.Finished || m.State == Core.Inbox.Enums.InboxState.Failed)
                    && m.UpdatedAt < cutoffDate)
            .OrderBy(m => m.UpdatedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteMessagesAsync(IEnumerable<InboxMessage> messages, CancellationToken cancellationToken = default)
    {
        _dbContextWrapper.Context.RemoveRange(messages);
        return await _dbContextWrapper.Context.SaveChangesAsync(cancellationToken);
    }
}
