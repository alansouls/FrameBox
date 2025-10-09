using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrameBox.Core.Outbox.Defaults;

internal class OutboxTimeoutService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeProvider _timeProvider;
    private readonly PeriodicTimer _timer;

    public OutboxTimeoutService(IServiceProvider serviceProvider, TimeProvider timeProvider)
    {
        _serviceProvider = serviceProvider;
        _timeProvider = timeProvider;
        _timer = new PeriodicTimer(InternalOutboxOptions.TimeoutCheckerFrequency);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var outboxStorage = scope.ServiceProvider.GetRequiredService<IOutboxStorage>();
                var messages = await outboxStorage.GetMessagesToTimeoutAsync(InternalOutboxOptions.MaxBatchCountToTimeout, stoppingToken);

                foreach (var message in messages.Where(m => m.UpdatedAt + InternalOutboxOptions.Timeout < _timeProvider.GetUtcNow()))
                {
                    message.Fail(_timeProvider.GetUtcNow());
                }

                await outboxStorage.UpdateMessagesAsync(messages, stoppingToken);
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
