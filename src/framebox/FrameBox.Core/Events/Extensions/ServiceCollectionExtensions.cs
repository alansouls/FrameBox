using FrameBox.Core.Events.Defaults;
using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Core.Events.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventDefaultServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IEventRegistry, EventRegistry>();
        services.TryAddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();
        services.TryAddScoped<IEventHandlerProvider, DefaultEventHandlerProvider>();
        services.TryAddScoped<IEventDispatcher, DefaultEventDispatcher>();

        return services;
    }

    public static IServiceCollection RegisterEventsFromAssemblyContaining<TType>(this IServiceCollection services,
        Func<IServiceProvider, Type, string>? eventNameFactory = null)
    {
        var assembly = typeof(TType).Assembly;
        var eventTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i == typeof(IEvent)))
            .ToArray();

        var eventHolder = new EventHolder
        {
            EventTypes = eventTypes
        };

        if (eventNameFactory is not null)
        {
            eventHolder.GetEventNameFunc = eventNameFactory;
        }

        services.AddSingleton(eventHolder);

        return services;
    }

    public static IServiceCollection AddEventHandlersFromAssemblyContaining<TType>(this IServiceCollection services, 
        Func<IServiceProvider, Type, string>? eventHandlerNameFactory = null)
    {
        var assembly = typeof(TType).Assembly;

        var eventHandlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
            .ToArray();

        foreach (var eventHandlerType in eventHandlerTypes)
        {
            services.AddEventHandler(eventHandlerType);
        }

        var handlerHolder = new EventHandlerHolder
        {
            EventHandlerTypes = eventHandlerTypes
        };

        if (eventHandlerNameFactory is not null)
        {
            handlerHolder.GetHandlerNameFunc = eventHandlerNameFactory;
        }

        services.AddSingleton(handlerHolder);

        services.TryAddSingleton<IEventRegistry, EventRegistry>();

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
