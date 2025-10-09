using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;

namespace FrameBox.Core.Outbox.Defaults;

internal class DefaultOutboxMessageHandler : IOutboxHandler
{
    private readonly IInboxMessageFactory _inboxMessageFactory;
    private readonly IInboxStorage _inboxStorage;
    private readonly IDomainEventSerializer _domainEventSerializer;
    private readonly IMessageBroker _messageBroker;
    private readonly IEventRegistry _eventRegistry;

    public DefaultOutboxMessageHandler(IInboxMessageFactory inboxMessageFactory, IInboxStorage inboxStorage, IDomainEventSerializer domainEventSerializer, IMessageBroker messageBroker, IEventRegistry eventRegistry)
    {
        _inboxMessageFactory = inboxMessageFactory;
        _inboxStorage = inboxStorage;
        _domainEventSerializer = domainEventSerializer;
        _messageBroker = messageBroker;
        _eventRegistry = eventRegistry;
    }

    public async Task HandleMessage(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        if (await _inboxStorage.ExistsInboxByOutboxIdAsync(outboxMessage.Id, cancellationToken))
        {
            return;
        }

        var domainEvent = await outboxMessage.ToDomainEvent(_eventRegistry, _domainEventSerializer, cancellationToken);

        var inboxMessages = _inboxMessageFactory.CreateMessages(domainEvent)
            .ToList();

        if (inboxMessages.Count == 0)
        {
            return;
        }

        await _inboxStorage.AddMessagesAsync(inboxMessages, cancellationToken);
        await _messageBroker.SendMessagesAsync(inboxMessages, CancellationToken.None);
    }
}
