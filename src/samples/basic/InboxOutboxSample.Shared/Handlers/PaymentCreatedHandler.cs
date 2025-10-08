using FrameBox.Core.Events.Interfaces;
using InboxOutboxSample.ApiService.Domain;
using InboxOutboxSample.Shared.Utils;
using Microsoft.Extensions.Logging;

namespace InboxOutboxSample.Shared.Handlers;

internal class PaymentCreatedHandler : FrameBox.Core.Events.Defaults.EventHandler<PaymentCreatedEvent>
{
    private readonly ILogger<PaymentCreatedHandler> _logger;
    private readonly UsefulDataBag _bag;

    public PaymentCreatedHandler(ILogger<PaymentCreatedHandler> logger, UsefulDataBag bag)
    {
        _logger = logger;
        _bag = bag;
    }

    public override Task HandleAsync(PaymentCreatedEvent @event, CancellationToken cancellationToken)
    {
        var ipAddress = _bag.IpAddress ?? "Unknown";

        var chance = Random.Shared.Next(0, 2);

        if (chance == 0)
        {
            _logger.LogInformation("Success, we had a payment with Id: {} - Value: {} by {IpAddress}", @event.PaymentId, @event.Amount, ipAddress);
            return Task.CompletedTask;
        }
        else
        {
            _logger.LogError("Error, something went wrong with payment Id: {} - Value: {} by {IpAddress}", @event.PaymentId, @event.Amount, ipAddress);
            throw new Exception("Simulated failure in PaymentCreatedHandler");
        }
    }
}
