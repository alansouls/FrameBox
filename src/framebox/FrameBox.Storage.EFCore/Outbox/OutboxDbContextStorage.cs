using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using FrameBox.Storage.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FrameBox.Storage.EFCore.Outbox;

internal class OutboxDbContextStorage : IOutboxStorage
{
    private readonly InternalDbContextWrapper<IOutboxStorage> _dbContextWrapper;
    private readonly TimeProvider _timeProvider;

    public OutboxDbContextStorage(InternalDbContextWrapper<IOutboxStorage> dbContextWrapper, TimeProvider timeProvider)
    {
        _dbContextWrapper = dbContextWrapper;
        _timeProvider = timeProvider;
    }

    public async Task AddMessagesAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default)
    {
        await _dbContextWrapper.Context.AddRangeAsync(messages, cancellationToken);

        await _dbContextWrapper.Context.SaveChangesAsync(cancellationToken);
    }

    public Task<OutboxMessage?> GetMessageByIdAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return _dbContextWrapper.Context.Set<OutboxMessage>().FirstOrDefaultAsync(m => m.EventId == messageId, cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetMessagesToSendAsync(int maxCount, CancellationToken cancellationToken = default)
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
                .Set<OutboxMessage>()
                .Where(m => m.State == Core.Outbox.Enums.OutboxState.Pending || m.State == Core.Outbox.Enums.OutboxState.ReadyToRetry)
                .OrderBy(m => m.UpdatedAt)
                .Take(maxCount);

            await messagesToUpdateQuery.ExecuteUpdateAsync(m => m.SetProperty(x => x.State, Core.Outbox.Enums.OutboxState.Sending)
                .SetProperty(x => x.UpdatedAt, timeStamp).SetProperty(x => x.ProcessId, processId), cancellationToken);

            var messages = await _dbContextWrapper.Context
                .Set<OutboxMessage>()
                .Where(m => m.ProcessId == processId)
                .ToListAsync(cancellationToken);

            return messages;
        }, verifySucceeded: (ct) => _dbContextWrapper.Context.Set<OutboxMessage>().AnyAsync(o => o.ProcessId == o.ProcessId, cancellationToken: ct), isolationLevel: System.Data.IsolationLevel.RepeatableRead, cancellationToken);
    }

    public async Task<IEnumerable<OutboxMessage>> GetMessagesToTimeoutAsync(int maxCount, CancellationToken cancellationToken = default)
    {
        if (maxCount <= 0)
        {
            return [];
        }

        return await _dbContextWrapper.Context.Set<OutboxMessage>()
            .Where(m => m.State == Core.Outbox.Enums.OutboxState.Sending)
            .OrderBy(m => m.UpdatedAt)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateMessagesAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken = default)
    {
        _dbContextWrapper.Context.UpdateRange(messages);
        await _dbContextWrapper.Context.SaveChangesAsync(cancellationToken);
    }
}
