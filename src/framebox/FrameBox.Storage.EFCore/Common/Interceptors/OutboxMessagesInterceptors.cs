using FrameBox.Core.Common.Entities;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FrameBox.Storage.EFCore.Common.Interceptors;

internal class OutboxMessagesInterceptors : SaveChangesInterceptor
{
    private List<OutboxMessage>? _messages = null;
    private readonly IOutboxMessageFactory _messageFactory;
    private readonly IMessageBroker _messageBroker;

    public OutboxMessagesInterceptors(IOutboxMessageFactory messageFactory, IMessageBroker messageBroker)
    {
        _messageFactory = messageFactory;
        _messageBroker = messageBroker;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entities = eventData.Context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified))
            .Select(e => (e.Entity as BaseEntity)!)
            .ToList();

        var events = entities.SelectMany(e => e.ConsumeEvents()).ToList();

        if (events.Count == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var messages = await Task.WhenAll(events.Select(async e => await _messageFactory.CreateAsync(e, cancellationToken)));

        await eventData.Context.AddRangeAsync(messages, cancellationToken);

        _messages = [.. messages];

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return Task.Run(async () => await SavingChangesAsync(eventData, result)).GetAwaiter().GetResult();
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if ((_messages ?? []).Count != 0)
        {
            await _messageBroker.SendMessagesAsync(_messages!, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        return Task.Run(async () => await SavedChangesAsync(eventData, result)).GetAwaiter().GetResult();
    }
}
