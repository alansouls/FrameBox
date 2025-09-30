using FrameBox.Core.Events.Defaults;
using FrameBox.Core.Events.Interfaces;

namespace InboxOutboxSample.ApiService.Domain;

public record PaymentCreatedEvent(Guid PaymentId, decimal Amount) : FrameEvent
{
    public override Guid Id => PaymentId;
}
