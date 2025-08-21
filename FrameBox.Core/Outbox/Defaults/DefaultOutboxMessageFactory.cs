using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.Core.Outbox.Defaults;

internal class DefaultOutboxMessageFactory : IOutboxMessageFactory
{
    private readonly TimeProvider _timeProvider;
    private readonly IDomainEventSerializer _domainEventSerializer;

    public DefaultOutboxMessageFactory(TimeProvider timeProvider,
        IDomainEventSerializer domainEventSerializer)
    {
        _timeProvider = timeProvider;
        _domainEventSerializer = domainEventSerializer;
    }

    public async ValueTask<OutboxMessage> CreateAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var eventPayload = await _domainEventSerializer.SerializeAsync(domainEvent, cancellationToken);

        return new OutboxMessage(domainEvent.Id, domainEvent.EventName, eventPayload, _timeProvider.GetUtcNow());
    }
}
