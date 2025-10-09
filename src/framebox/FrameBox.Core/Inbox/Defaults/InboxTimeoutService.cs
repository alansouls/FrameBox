using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrameBox.Core.Inbox.Defaults;

internal class InboxTimeoutService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeProvider _timeProvider;
    private readonly PeriodicTimer _timer;

    public InboxTimeoutService(IServiceProvider serviceProvider, TimeProvider timeProvider)
    {
        _serviceProvider = serviceProvider;
        _timeProvider = timeProvider;
        _timer = new PeriodicTimer(InternalInboxOptions.TimeoutCheckerFrequency);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var inboxStorage = scope.ServiceProvider.GetRequiredService<IInboxStorage>();
                var messages = await inboxStorage.GetMessagesToTimeoutAsync(InternalInboxOptions.MaxBatchCountToTimeout, stoppingToken);

                var timedoutMessages = messages.Where(m => m.UpdatedAt + InternalInboxOptions.Timeout < _timeProvider.GetUtcNow())
                    .ToList();

                foreach (var message in timedoutMessages)
                {
                    message.Fail("Timeout", _timeProvider.GetUtcNow());
                }

                await inboxStorage.UpdateMessagesAsync(timedoutMessages, stoppingToken);
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
