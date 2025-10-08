using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Services;
using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Core.EventContexts.Builders;

public class EventContextFeederRegistryBuilder
{
    private readonly IServiceCollection _services;

    public EventContextFeederRegistryBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public EventContextFeederRegistryBuilder AddFeeder<TFactory, TEvent>()
        where TFactory : class, IEventContextFeeder
        where TEvent : class, IEvent
    {
        _services.TryAddScoped<TFactory>();
        _services.AddScoped(provider => new EventContextFactoryRegistry(provider.GetRequiredService<TFactory>(), typeof(TEvent)));
        return this;
    }

    public EventContextFeederRegistryBuilder AddFeeder<TFactory>(params IEnumerable<Type> eventTypes)
        where TFactory : class, IEventContextFeeder
    {
        if (eventTypes.Any(e => !typeof(IEvent).IsAssignableFrom(e)))
        {
            throw new ArgumentException("All event types must implement IEvent interface.", nameof(eventTypes));
        }

        _services.TryAddScoped<TFactory>();
        _services.AddScoped(provider => new EventContextFactoryRegistry(provider.GetRequiredService<TFactory>(), eventTypes));
        return this;
    }

    public EventContextFeederRegistryBuilder AddFeeder<TFactory>()
        where TFactory : class, IEventContextFeeder
    {
        _services.TryAddScoped<TFactory>();
        _services.AddScoped(provider => new EventContextFactoryRegistry(provider.GetRequiredService<TFactory>()));
        return this;
    }

    public EventContextFeederRegistryBuilder AddRestorer<TRestorer>(string type)
        where TRestorer : class, IEventContextRestorer
    {
        _services.AddKeyedScoped<IEventContextRestorer, TRestorer>(type);

        return this;
    }
}
