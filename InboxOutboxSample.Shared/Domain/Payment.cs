using FrameBox.Core.Common.Entities;

namespace InboxOutboxSample.ApiService.Domain;

public class Payment : BaseEntity
{
    public Guid Id { get; }

    public decimal Amount { get; }

    public Payment(decimal amount)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        RaiseEvent(new PaymentCreatedEvent(Id, amount));
    }
}
