using FrameBox.Core.Outbox.Enums;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrameBox.Core.Outbox.Defaults;

internal class OutboxCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeProvider _timeProvider;
    private readonly PeriodicTimer _timer;

    public OutboxCleanupService(IServiceProvider serviceProvider, TimeProvider timeProvider)
    {
        _serviceProvider = serviceProvider;
        _timeProvider = timeProvider;
        _timer = new PeriodicTimer(InternalOutboxOptions.CleanupFrequency);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxStorage = scope.ServiceProvider.GetRequiredService<IOutboxStorage>();
                
                var cutoffDate = _timeProvider.GetUtcNow().AddDays(-InternalOutboxOptions.RetentionPeriodInDays);
                
                var messages = await outboxStorage.GetMessagesToCleanupAsync(InternalOutboxOptions.MaxBatchCountToCleanup, cutoffDate, stoppingToken);

                var messagesToDelete = messages.ToList();

                if (messagesToDelete.Count > 0)
                {
                    await outboxStorage.DeleteMessagesAsync(messagesToDelete, stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
            }
        }
    }
}
