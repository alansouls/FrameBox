using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace FrameBox.Core.Inbox.Defaults;

internal class DefaultInboxHandler : IInboxHandler
{
    private readonly IOutboxStorage _outboxStorage;
    private readonly IEventHandlerProvider _eventHandlerProvider;
    private readonly IInboxStorage _inboxStorage;
    private readonly ILogger<DefaultInboxHandler> _logger;
    private readonly IDomainEventSerializer _domainEventSerializer;
    private readonly TimeProvider _timeProvider;

    public DefaultInboxHandler(IOutboxStorage outboxStorage, IEventHandlerProvider provider, IInboxStorage inboxStorage, ILogger<DefaultInboxHandler> logger, IDomainEventSerializer domainEventSerializer, TimeProvider timeProvider)
    {
        _outboxStorage = outboxStorage;
        _eventHandlerProvider = provider;
        _inboxStorage = inboxStorage;
        _logger = logger;
        _domainEventSerializer = domainEventSerializer;
        _timeProvider = timeProvider;
    }

    public async Task HandleMessage(InboxMessage message, CancellationToken cancellationToken)
    {
        var outboxMessage = await _outboxStorage.GetMessageByIdAsync(message.OutboxMessageId, cancellationToken);

        if (outboxMessage is null)
        {
            _logger.LogWarning("Outbox messages with ID {OutboxMessageId} not found for inbox messages {InboxMessageId}. Skipping.",
                message.OutboxMessageId, message.Id);

            return;
        }

        try
        {
            var domainEvent = await outboxMessage.ToDomainEvent(_domainEventSerializer, cancellationToken);
            var handler = _eventHandlerProvider.GetEventHandler(message.HandlerName);
            message.Start(_timeProvider.GetUtcNow());
            await _inboxStorage.UpdateMessagesAsync([message], cancellationToken);
            await handler.HandleAsync(domainEvent, cancellationToken);
            message.Complete(_timeProvider.GetUtcNow());
            await _inboxStorage.UpdateMessagesAsync([message], cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process inbox messages {InboxMessageId} linked to outbox messages {OutboxMessageId}.",
                message.Id, message.OutboxMessageId);

            var messagePayload = JsonSerializer.Serialize(new
            {
                ex.Message,
                ex.StackTrace
            });

            message.Fail(messagePayload, _timeProvider.GetUtcNow());
            await _inboxStorage.UpdateMessagesAsync([message], CancellationToken.None);
        }
    }
}
