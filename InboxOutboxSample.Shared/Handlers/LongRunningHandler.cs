using InboxOutboxSample.ApiService.Domain;
using Microsoft.Extensions.Logging;

namespace InboxOutboxSample.Shared.Handlers;

internal class LongRunningHandler : FrameBox.Core.Events.Defaults.EventHandler<PaymentCreatedEvent>
{
    private readonly ILogger<LongRunningHandler> _logger;

    public LongRunningHandler(ILogger<LongRunningHandler> logger)
    {
        _logger = logger;
    }

    public override async Task HandleAsync(PaymentCreatedEvent @event, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(30));

        _logger.LogInformation("Long running handler processed PaymentCreatedEvent with Id: {EventId}", @event.Id);
    }
}
