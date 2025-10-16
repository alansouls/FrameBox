using Azure.Messaging.ServiceBus;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.AzureServiceBus.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace FrameBox.MessageBroker.AzureServiceBus.Services;

internal class AzureServiceBusListener : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public AzureServiceBusListener(IServiceProvider serviceProvider)
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

        var options = scope.ServiceProvider.GetRequiredService<IOptions<AzureServiceBusOptions>>().Value;
        var client = string.IsNullOrEmpty(options.ConnectionKey) ?
            scope.ServiceProvider.GetRequiredService<ServiceBusClient>() :
            scope.ServiceProvider.GetRequiredKeyedService<ServiceBusClient>(options.ConnectionKey);

        var listener = options.IsTopicSubscription<TMessage>() ?
            client.CreateReceiver(options.GetTopicName<TMessage>(), options.GetSubscriptionName<TMessage>()) :
            client.CreateReceiver(options.GetQueueName<TMessage>());

        int runningMessages = 0;

        await foreach (var serviceBusMessage in listener.ReceiveMessagesAsync(cancellationToken))
        {
            while (runningMessages >= options.GetMaxConcurrency<TMessage>())
            {
                //TODO: is there a better way to do this?
                await Task.Delay(100, cancellationToken);
            }

            Interlocked.Increment(ref runningMessages);
            _ = Task.Run(async () =>
            {
                var message = deserializer(serviceBusMessage.Body.ToArray());
                using var messageScope = _serviceProvider.CreateScope();
                var handler = messageScope.ServiceProvider.GetRequiredService<TMessageHandler>();
                await handler.HandleMessage(message, cancellationToken);
                await listener.CompleteMessageAsync(serviceBusMessage, CancellationToken.None);
                Interlocked.Decrement(ref runningMessages);
            }, CancellationToken.None);
        }
    }
}
