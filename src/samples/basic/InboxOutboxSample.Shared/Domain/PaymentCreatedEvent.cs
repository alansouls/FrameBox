using FrameBox.Core.Events.Interfaces;

namespace InboxOutboxSample.ApiService.Domain;

public record PaymentCreatedEvent(Guid PaymentId, decimal Amount) : IDomainEvent
{
    public Guid Id => PaymentId;

    public DateTimeOffset RaisedAt { get; init; } = DateTimeOffset.UtcNow;
}
