using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Services;
using FrameBox.MessageBroker.RabbitMQ.EventContexts;
using FrameBox.MessageBroker.RabbitMQ.Options;
using FrameBox.MessageBroker.RabbitMQ.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.MessageBroker.RabbitMQ.Extensions;

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
        services.TryAddScoped<MessageHeaderHolder>();

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
        services.TryAddScoped<MessageHeaderHolder>();

        return services;
    }

    /// <summary>
    /// Adds an EventContextStorage implementation that feeds context from message headers
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMQMessageHeaderEventContextStorage(this IServiceCollection services)
    {
        services.TryAddScoped<IEventContextStorage, MessageHeaderEventContextStorage>();
        services.TryAddScoped<IEventContextStorageFactory, DefaultEventContextStorageFactory>();

        return services;
    }
}
