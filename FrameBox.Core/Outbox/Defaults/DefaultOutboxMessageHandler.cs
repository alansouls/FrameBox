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
    private readonly IOutboxStorage _outboxStorage;
    private readonly TimeProvider _timeProvider;

    public DefaultOutboxMessageHandler(IInboxMessageFactory inboxMessageFactory, IInboxStorage inboxStorage, IDomainEventSerializer domainEventSerializer, IMessageBroker messageBroker, IOutboxStorage outboxStorage, TimeProvider timeProvider)
    {
        _inboxMessageFactory = inboxMessageFactory;
        _inboxStorage = inboxStorage;
        _domainEventSerializer = domainEventSerializer;
        _messageBroker = messageBroker;
        _outboxStorage = outboxStorage;
        _timeProvider = timeProvider;
    }

    public async Task HandleMessage(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        if (await _inboxStorage.ExistsInboxByOutboxIdAsync(outboxMessage.Id, cancellationToken))
        {
            await ReceiveOutboxMessage(outboxMessage);
            return;
        }

        var domainEvent = await outboxMessage.ToDomainEvent(_domainEventSerializer, cancellationToken);

        var inboxMessages = _inboxMessageFactory.CreateMessages(domainEvent)
            .ToList();

        if (inboxMessages.Count == 0)
        {
            await ReceiveOutboxMessage(outboxMessage);
            return;
        }

        await _inboxStorage.AddMessagesAsync(inboxMessages, cancellationToken);
        await ReceiveOutboxMessage(outboxMessage);
        await _messageBroker.SendMessagesAsync(inboxMessages, CancellationToken.None);
    }

    private async Task ReceiveOutboxMessage(OutboxMessage outboxMessage)
    {
        outboxMessage.SetAsReceived(_timeProvider.GetUtcNow());
        await _outboxStorage.UpdateMessagesAsync([outboxMessage], CancellationToken.None);
    }
}
