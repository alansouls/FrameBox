using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.AzureServiceBus.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace FrameBox.MessageBroker.AzureServiceBus.Services;

internal class InProcessListener : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public InProcessListener(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.WhenAll(Task.Run(async () => await ListenToOutboxMessages(stoppingToken), stoppingToken),
            Task.Run(async () => await ListenToInboxMessages(stoppingToken), stoppingToken));
    }

    private Task ListenToOutboxMessages(CancellationToken cancellationToken)
    {
        return ListenToMessages<OutboxMessage, IOutboxHandler>(
            (data) => OutboxMessage.FromJson(data),
            cancellationToken);
    }

    private Task ListenToInboxMessages(CancellationToken cancellationToken)
    {
        return ListenToMessages<InboxMessage, IInboxHandler>(
            (data) => InboxMessage.FromJson(data),
            cancellationToken);
    }

    private async Task ListenToMessages<TMessage, TMessageHandler>(
        Func<ReadOnlySpan<byte>, TMessage> deserializer,
        CancellationToken cancellationToken) where TMessage : class, IMessage
        where TMessageHandler : IMessageHandler<TMessage>
    {
        using var scope = _serviceProvider.CreateScope();

        var options = scope.ServiceProvider.GetRequiredService<IOptions<InProcessOptions>>().Value;
        var channelReader = scope.ServiceProvider.GetRequiredService<ChannelReader<TMessage>>();

        int runningMessages = 0;

        await foreach (var message in channelReader.ReadAllAsync(cancellationToken))
        {
            while (runningMessages >= options.GetMaxConcurrency<TMessage>())
            {
                //TODO: is there a better way to do this?
                await Task.Delay(100, cancellationToken);
            }

            Interlocked.Increment(ref runningMessages);
            _ = Task.Run(async () =>
            {
                using var messageScope = _serviceProvider.CreateScope();
                var handler = messageScope.ServiceProvider.GetRequiredService<TMessageHandler>();
                await handler.HandleMessage(message, cancellationToken);
                Interlocked.Decrement(ref runningMessages);
            }, CancellationToken.None);
        }
    }
}
