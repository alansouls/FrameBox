using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Inbox.Options;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using FrameBox.Core.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FrameBox.Core.Inbox.Defaults;

internal class InboxDispatcher : IInboxDispatcher, IHostedService
{
    private readonly PeriodicTimer _timer;
    private readonly CancellationTokenSource _timerCancellationTokenSource;
    private readonly IServiceProvider _serviceProvider;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
    private readonly ILogger<InboxDispatcher> _logger;

    public InboxDispatcher(IServiceProvider serviceProvider, ILogger<InboxDispatcher> logger, TimeProvider timeProvider)
    {
        _timer = new PeriodicTimer(InternalOutboxOptions.RetryInterval);
        _timerCancellationTokenSource = new CancellationTokenSource();
        _serviceProvider = serviceProvider;
        _logger = logger;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dispatching outbox messages - {Message}", ex.Message);
            }
        }

        _logger.LogInformation("Inbox dispatcher stopped.");
    }

    public Task DispatchFromOutboxMessage(OutboxMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task DispatchMessagesAsync(CancellationToken cancellationToken = default)
    {
        await _semaphoreSlim.WaitAsync(cancellationToken);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var inboxStorage = scope.ServiceProvider.GetRequiredService<IInboxStorage>();
            var eventHandlerProvider = scope.ServiceProvider.GetRequiredService<IEventHandlerProvider>();
            var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
            var messages = await inboxStorage.GetMessagesReadyToSendAsync(InternalInboxOptions.MaxBatchCountToRetry, cancellationToken);

            var inboxMessages = messages.ToList();
            
            if (inboxMessages.Count == 0)
            {
                return;
            }

            await inboxStorage.UpdateMessagesAsync(inboxMessages, cancellationToken);
            await messageBroker.SendMessagesAsync(inboxMessages, cancellationToken);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public void RunNow()
    {
        _ = Task.Run(async () => await DispatchMessagesAsync(CancellationToken.None));
    }
}
