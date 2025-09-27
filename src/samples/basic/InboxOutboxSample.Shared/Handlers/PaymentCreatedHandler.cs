using FrameBox.Core.Events.Interfaces;
using InboxOutboxSample.ApiService.Domain;
using Microsoft.Extensions.Logging;

namespace InboxOutboxSample.Shared.Handlers;

internal class PaymentCreatedHandler : FrameBox.Core.Events.Defaults.EventHandler<PaymentCreatedEvent>
{
    private readonly ILogger<PaymentCreatedHandler> _logger;

    public PaymentCreatedHandler(ILogger<PaymentCreatedHandler> logger)
    {
        _logger = logger;
    }

    public override Task HandleAsync(PaymentCreatedEvent @event, CancellationToken cancellationToken)
    {
        var chance = Random.Shared.Next(0, 2);

        if (chance == 0)
        {
            _logger.LogInformation("Success, we had a payment with Id: {} - Value: {}", @event.PaymentId, @event.Amount);
            return Task.CompletedTask;
        }
        else
        {
            _logger.LogError("Error, something went wrong with payment Id: {} - Value: {}", @event.PaymentId, @event.Amount);
            throw new Exception("Simulated failure in PaymentCreatedHandler");
        }
    }
}
