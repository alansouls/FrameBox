using FrameBox.Core.Common.Interfaces;
using FrameBox.MessageBroker.AzureServiceBus.Options;
using FrameBox.MessageBroker.AzureServiceBus.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.MessageBroker.RabbitMQ.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Requires a ServiceBusClient to be registered in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureServiceBusMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureServiceBusOptions>(options => configuration.GetSection(nameof(AzureServiceBusOptions)).Bind(options));
        services.AddScoped<IMessageBroker, AzureServiceBusBroker>();

        return services;
    }

    /// <summary>
    /// Requires a ServiceBusClient to be registered in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAzureServiceBusListener(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AzureServiceBusOptions>(options => configuration.GetSection(nameof(AzureServiceBusOptions)).Bind(options));
        services.AddHostedService<AzureServiceBusListener>();

        return services;
    }
}
