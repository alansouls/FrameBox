using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Events.Defaults;

public sealed class EventHolder
{
    public Type[] EventTypes { get; set; } = [];

    public Func<IServiceProvider, Type, string> GetEventNameFunc = (_, type) => type.FullName ?? throw new InvalidDataException("Invalid event type");
}

public sealed class EventHandlerHolder
{
    public Type[] EventHandlerTypes { get; set; } = [];

    public Func<IServiceProvider, Type, string> GetHandlerNameFunc = (_, type) => type.FullName ?? throw new InvalidDataException("Invalid event handler type");
}

public sealed class EventRegistry : IEventRegistry
{
    private readonly Dictionary<string, Type> _eventTypes = new();
    private readonly Dictionary<string, Type> _eventHandlerTypes = new();
    private readonly Dictionary<Type, string> _eventNames = new();
    private readonly Dictionary<Type, string> _eventHandlerNames = new();
    private readonly IEnumerable<EventHolder> _eventHolders;
    private readonly IEnumerable<EventHandlerHolder> _eventHandlerHolders;

    public EventRegistry(IEnumerable<EventHolder> eventHolders, IEnumerable<EventHandlerHolder> eventHandlerHolders)
    {
        _eventHolders = eventHolders;
        _eventHandlerHolders = eventHandlerHolders;

        foreach (var holder in _eventHolders)
        {
            foreach (var eventType in holder.EventTypes)
            {
                var eventName = holder.GetEventNameFunc.Invoke(null!, eventType);
                _eventTypes[eventName] = eventType;
                _eventNames[eventType] = eventName;
            }
        }

        foreach (var holder in _eventHandlerHolders)
        {
            foreach (var handlerType in holder.EventHandlerTypes)
            {
                var handlerName = holder.GetHandlerNameFunc.Invoke(null!, handlerType);
                _eventHandlerTypes[handlerName] = handlerType;
                _eventHandlerNames[handlerType] = handlerName;
            }
        }
    }

    public Type? GetEventHandlerType(string handlerName)
    {
        return _eventHandlerTypes.GetValueOrDefault(handlerName);
    }

    public Type? GetEventType(string eventName)
    {
        return _eventTypes.GetValueOrDefault(eventName);
    }

    public string GetEventName(Type eventType)
    {
        return _eventNames.GetValueOrDefault(eventType) ?? throw new InvalidDataException("Event type not registered");
    }

    public string GetHandlerName(Type handlerType)
    {
        return _eventHandlerNames.GetValueOrDefault(handlerType) ?? throw new InvalidDataException("Event handler type not registered");
    }
}
