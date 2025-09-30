using InboxOutboxSample.Shared.Domain;
using Microsoft.Extensions.Logging;

namespace InboxOutboxSample.Shared.Handlers;

public class NonDomainEventHandler : FrameBox.Core.Events.Defaults.EventHandler<NonDomainEvent>
{
    private readonly ILogger<NonDomainEventHandler> _logger;

    public NonDomainEventHandler(ILogger<NonDomainEventHandler> logger)
    {
        _logger = logger;
    }

    public override Task HandleAsync(NonDomainEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("This is a non-domain event with message: {Hello}", @event.Hello);

        return Task.CompletedTask;
    }
}