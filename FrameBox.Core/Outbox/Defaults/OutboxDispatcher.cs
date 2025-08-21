using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using FrameBox.Core.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FrameBox.Core.Outbox.Defaults;

internal class OutboxDispatcher : IOutboxDispatcher, IHostedService
{
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _timerCancellationTokenSource;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxDispatcher> _logger;
    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    private readonly TimeProvider _timeProvider;

    public OutboxDispatcher(IServiceProvider serviceProvider, ILogger<OutboxDispatcher> logger, TimeProvider timeProvider)
    {
        _timer = new PeriodicTimer(InternalOutboxOptions.DispatchInterval);
        _timerCancellationTokenSource = new CancellationTokenSource();
        _serviceProvider = serviceProvider;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public void RunNow()
    {
        _ = Task.Run(async () => await DispatchMessagesAsync(CancellationToken.None));
    }

    private async Task DispatchMessagesAsync(CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var outboxStorage = scope.ServiceProvider.GetRequiredService<IOutboxStorage>();
            var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
            var messages = await outboxStorage.GetMessagesAsync(InternalOutboxOptions.MaxMessagesPerDispatch, cancellationToken);
            
            try
            {
                await messageBroker.SendMessagesAsync(messages, cancellationToken);
            }
            catch (FailedToSendMessagesException<OutboxMessage> ex)
            {
                _logger.LogError(ex, "Failed to send outbox messages.");

                foreach (var message in ex.FailedMessages)
                {
                    message.SetAsPending(_timeProvider.GetUtcNow());
                }

                messages = messages.Except(ex.FailedMessages);
            }

            foreach (var message in messages)
            {
                message.SetAsSent(_timeProvider.GetUtcNow());
            }

            await outboxStorage.UpdateMessagesAsync(messages, cancellationToken);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () => await DispatchMessagesOnTimerAsync(_timerCancellationTokenSource.Token), cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _timerCancellationTokenSource.CancelAsync();
        _timer.Dispose();
    }

    private async Task DispatchMessagesOnTimerAsync(CancellationToken cancellationToken)
    {
        while (await _timer.WaitForNextTickAsync(CancellationToken.None))
        {
            try
            {
                await DispatchMessagesAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }

        _logger.LogInformation("Outbox dispatcher stopped.");
    }
}
