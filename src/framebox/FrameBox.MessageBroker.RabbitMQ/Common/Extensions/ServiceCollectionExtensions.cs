using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.RabbitMQ.Common.Defaults;
using FrameBox.MessageBroker.RabbitMQ.Common.Interfaces;
using FrameBox.MessageBroker.RabbitMQ.Common.Options;
using FrameBox.MessageBroker.RabbitMQ.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.MessageBroker.RabbitMQ.Common.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Requires a RabbitMQ IConnection to be registered in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMQMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQOptions>(options => configuration.GetSection(nameof(RabbitMQOptions)).Bind(options));
        services.AddScoped<IMessageBroker, RabbitMQBroker>();
        
        services.TryAddScoped<IRoutingKeyFactory<OutboxMessage>, DefaultOutboxRoutingKeyFactory>();
        services.TryAddScoped<IRoutingKeyFactory<InboxMessage>, DefaultInboxRoutingKeyFactory>();

        return services;
    }

    /// <summary>
    /// Requires a RabbitMQ IConnection to be registered in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMQListener(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQOptions>(options => configuration.GetSection(nameof(RabbitMQOptions)).Bind(options));
        services.AddHostedService<RabbitMQListener>();

        return services;
    }
}
