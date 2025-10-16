using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.AzureServiceBus.Options;
using FrameBox.MessageBroker.AzureServiceBus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading.Channels;

namespace FrameBox.MessageBroker.RabbitMQ.Extensions;

public static class ServiceCollectionExtensions
{
    private static Channel<OutboxMessage>? _outboxChannel = null;
    private static Channel<InboxMessage>? _inboxChannel = null;
    private static Channel<OutboxMessage> OutboxChannel => _outboxChannel ??= Channel.CreateUnbounded<OutboxMessage>();
    private static Channel<InboxMessage> InboxChannel => _inboxChannel ??= Channel.CreateUnbounded<InboxMessage>();
    /// <summary>
    /// Requires a ServiceBusClient to be registered in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddInProcessMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(OutboxChannel.Writer);
        services.TryAddSingleton(InboxChannel.Writer);
        services.AddScoped<IMessageBroker, InProcessBroker>();

        return services;
    }

    /// <summary>
    /// Requires a ServiceBusClient to be registered in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddInProcessListener(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(OutboxChannel.Reader);
        services.TryAddSingleton(InboxChannel.Reader);
        services.AddHostedService<InProcessListener>();

        return services;
    }
}
