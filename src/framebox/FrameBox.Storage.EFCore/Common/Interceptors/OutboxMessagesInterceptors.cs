using FrameBox.Core.Common.Entities;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Storage.EFCore.Common.Interceptors;

internal class OutboxMessagesInterceptors : SaveChangesInterceptor
{
    private List<OutboxMessage>? _messages = null;
    private readonly IOutboxMessageFactory _messageFactory;
    private readonly IMessageBroker _messageBroker;
    private readonly TimeProvider _timeProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventContextManager _eventContextManager;

    public OutboxMessagesInterceptors(IOutboxMessageFactory messageFactory, IMessageBroker messageBroker,
        IEventContextManager eventContextManager,
        TimeProvider timeProvider, IServiceProvider serviceProvider)
    {
        _messageFactory = messageFactory;
        _messageBroker = messageBroker;
        _timeProvider = timeProvider;
        _serviceProvider = serviceProvider;
        _eventContextManager = eventContextManager;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        _messages = null;

        if (eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var entities = eventData.Context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity)
            .Select(e => (e.Entity as BaseEntity)!)
            .ToList();

        var events = entities.SelectMany(e => e.ConsumeEvents()).ToList();

        if (events.Count == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var eventContexts = (await _eventContextManager.CaptureContextsAsync(events, cancellationToken))
            .ToList();

        if (eventContexts.Count > 0)
        {
            using var eventContextScope = _serviceProvider.CreateScope();
            var scopedEventContextStorage = eventContextScope.ServiceProvider.GetRequiredService<IEventContextStorage>();
            await scopedEventContextStorage.AddAsync(eventContexts, cancellationToken);
        }

        var sendingDate = _timeProvider.GetUtcNow();

        var messages = await Task.WhenAll(events.Select(async e =>
        {
            var message = await _messageFactory.CreateAsync(e, cancellationToken);
            message.SetAsSending(sendingDate);
            return message;
        }));

        await eventData.Context.AddRangeAsync(messages, cancellationToken);

        _messages = [.. messages];

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return Task.Run(async () => await SavingChangesAsync(eventData, result)).GetAwaiter().GetResult();
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        if ((_messages ?? []).Count != 0)
        {
            await _messageBroker.SendMessagesAsync(_messages!, CancellationToken.None);

            await SaveSentMessagesAsync();
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        return Task.Run(async () => await SavedChangesAsync(eventData, result)).GetAwaiter().GetResult();
    }

    private async Task SaveSentMessagesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var scopedOutboxStorage = scope.ServiceProvider.GetRequiredService<IOutboxStorage>();

        var sentDate = _timeProvider.GetUtcNow();

        foreach (var message in _messages!)
        {
            message.SetAsSent(sentDate);
        }

        await scopedOutboxStorage.UpdateMessagesAsync(_messages!, CancellationToken.None);
    }
}