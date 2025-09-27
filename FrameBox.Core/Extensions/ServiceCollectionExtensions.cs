using FrameBox.Core.Events.Extensions;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Extensions;
using FrameBox.Core.Outbox.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameBoxCore(this IServiceCollection services)
    {
        services.AddOutboxServices();
        services.AddInboxServices();
        services.AddEventDefaultServices();

        return services;
    }

    public static IServiceCollection AddEventHandlersFromAssemblyContaining<TType>(this IServiceCollection services)
    {
        var assembly = typeof(TType).Assembly;

        var eventHandlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)));

        foreach (var eventHandlerType in eventHandlerTypes)
        {
            services.AddEventHandler(eventHandlerType);
        }

        return services;
    }

    private static IServiceCollection AddEventHandler(this IServiceCollection services, Type eventHandlerType)
    {
        var eventHandlerInterface = eventHandlerType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

        if (eventHandlerInterface is null)
        {
            throw new InvalidOperationException($"Type {eventHandlerType.FullName} does not implement IEventHandler<T> interface.");
        }

        return services.AddScoped(eventHandlerType)
            .AddScoped(eventHandlerInterface, provider => provider.GetRequiredService(eventHandlerType));
    }
}
