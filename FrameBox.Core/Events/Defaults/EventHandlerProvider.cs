using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.Events.Defaults;

public class EventHandlerProvider : IEventHandlerProvider
{
    private readonly IServiceProvider _serviceProvider;

    public EventHandlerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<IEventHandler> GetHandlers(IDomainEvent @event)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        
        return _serviceProvider.GetServices(handlerType)
            .Cast<IEventHandler>();
    }
}