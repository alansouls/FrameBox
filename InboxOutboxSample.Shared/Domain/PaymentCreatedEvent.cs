using FrameBox.Core.Events.Interfaces;

namespace InboxOutboxSample.ApiService.Domain;

public class PaymentCreatedEvent(Guid paymentId, decimal amount) : IDomainEvent
{
    public Guid Id => paymentId;

    public string EventName => nameof(PaymentCreatedEvent);

    public DateTimeOffset RaisedAt => DateTimeOffset.UtcNow;

    public decimal Amount { get; } = amount;
}
