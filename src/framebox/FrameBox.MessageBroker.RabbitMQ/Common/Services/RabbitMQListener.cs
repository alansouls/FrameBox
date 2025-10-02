using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Common.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text.Json;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Services;

internal class RabbitMQListener : IHostedService
{
    private readonly CancellationTokenSource _listenerCancellation = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly TaskCompletionSource _outboxListenerCompletionSource;
    private readonly TaskCompletionSource _inboxListenerCompletionSource;
    private readonly TaskCompletionSource _listenerCanceledSource;

    public RabbitMQListener(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _outboxListenerCompletionSource = new TaskCompletionSource();
        _inboxListenerCompletionSource = new TaskCompletionSource();
        _listenerCanceledSource = new TaskCompletionSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () => await ListenToOutboxMessages(_listenerCancellation.Token), cancellationToken);
        _ = Task.Run(async () => await ListenToInboxMessages(_listenerCancellation.Token), cancellationToken);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _listenerCancellation.CancelAsync();
        _listenerCanceledSource.SetResult();
        await _outboxListenerCompletionSource.Task;
        await _inboxListenerCompletionSource.Task;
    }

    private Task ListenToOutboxMessages(CancellationToken cancellationToken)
    {
        return ListenToMessages<OutboxMessage, IOutboxHandler>(
            (data) => OutboxMessage.FromJson(data),
            _outboxListenerCompletionSource,
            cancellationToken);
    }

    private Task ListenToInboxMessages(CancellationToken cancellationToken)
    {
        return ListenToMessages<InboxMessage, IInboxHandler>(
            (data) => InboxMessage.FromJson(data),
            _inboxListenerCompletionSource,
            cancellationToken);
    }

    private async Task ListenToMessages<TMessage, TMessageHandler>(
        Func<ReadOnlySpan<byte>, TMessage> deserializer,
        TaskCompletionSource listenerCompletionSource,
        CancellationToken cancellationToken) where TMessage : class, IMessage
        where TMessageHandler : IMessageHandler<TMessage>
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var connection = scope.ServiceProvider.GetRequiredService<IConnection>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

            var channel = await connection.CreateChannelAsync(new CreateChannelOptions(
                    publisherConfirmationsEnabled: false, publisherConfirmationTrackingEnabled: false,
                    consumerDispatchConcurrency: (ushort)options.GetMaxConcurrency<TMessage>()),
                cancellationToken); //TODO: check options

            var queueName = options.GetQueueName<TMessage>();

            await channel.QueueDeclareAsync(queueName, durable: true, autoDelete: false,
                cancellationToken: cancellationToken);

            var exchangeName = options.GetExchangeName<TMessage>();

            if (!string.IsNullOrEmpty(exchangeName))
            {
                await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic, durable: true,
                    cancellationToken: cancellationToken);

                await channel.QueueBindAsync(queueName, exchangeName, routingKey: options.GetTopicName<TMessage>(),
                    cancellationToken: cancellationToken);
            }

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                var message = deserializer(eventArgs.Body.Span);
                using var messageScope = _serviceProvider.CreateScope();
                var handler = messageScope.ServiceProvider.GetRequiredService<TMessageHandler>();
                await handler.HandleMessage(message, eventArgs.CancellationToken);
                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false,
                    cancellationToken: CancellationToken.None);
            };

            string consumerTag =
                await channel.BasicConsumeAsync(queueName, autoAck: false, consumer, cancellationToken);

            await _listenerCanceledSource.Task;

            await channel.BasicCancelAsync(consumerTag, cancellationToken: CancellationToken.None);
        }
        finally
        {
            listenerCompletionSource.SetResult();
        }
    }
}