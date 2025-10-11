using FrameBox.Core.Inbox.Enums;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrameBox.Core.Inbox.Defaults;

internal class InboxCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeProvider _timeProvider;
    private readonly PeriodicTimer _timer;

    public InboxCleanupService(IServiceProvider serviceProvider, TimeProvider timeProvider)
    {
        _serviceProvider = serviceProvider;
        _timeProvider = timeProvider;
        _timer = new PeriodicTimer(InternalInboxOptions.CleanupFrequency);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var inboxStorage = scope.ServiceProvider.GetRequiredService<IInboxStorage>();
                
                var cutoffDate = _timeProvider.GetUtcNow().AddDays(-InternalInboxOptions.RetentionPeriodInDays);
                
                var messages = await inboxStorage.GetMessagesToCleanupAsync(InternalInboxOptions.MaxBatchCountToCleanup, cutoffDate, stoppingToken);

                var messagesToDelete = messages.ToList();

                if (messagesToDelete.Count > 0)
                {
                    await inboxStorage.DeleteMessagesAsync(messagesToDelete, stoppingToken);
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
