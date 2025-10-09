using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.Events.Defaults;

internal class DefaultEventHandlerProvider : IEventHandlerProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEventRegistry _eventRegistry;

    public DefaultEventHandlerProvider(IServiceProvider serviceProvider, IEventRegistry eventRegistry)
    {
        _serviceProvider = serviceProvider;
        _eventRegistry = eventRegistry;
    }

    public IEventHandler GetEventHandler(string handlerName)
    {
        var handlerType = _eventRegistry.GetEventHandlerType(handlerName);

        if (handlerType is null)
        {
            throw new InvalidOperationException($"Event handler type '{handlerName}' not found.");
        }

        return (_serviceProvider.GetRequiredService(handlerType) as IEventHandler)!;
    }

    public IEnumerable<Type> GetEventHandlerTypes(Type eventType)
    {
        var handlerInterfaceType = typeof(IEventHandler<>).MakeGenericType(eventType);

        var handlerTypes = _serviceProvider.GetServices(handlerInterfaceType);

        return handlerTypes.Select(h => h!.GetType());
    }
}
