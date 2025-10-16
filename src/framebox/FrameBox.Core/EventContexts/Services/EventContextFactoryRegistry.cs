using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.EventContexts.Services;

public class EventContextFactoryRegistry
{
    public IEventContextFeeder Feeder { get; }

    public IEnumerable<Type> EventTypes { get; }

#if NET9_0_OR_GREATER
    public EventContextFactoryRegistry(IEventContextFeeder feeder, params IEnumerable<Type> eventTypes)
    {
        Feeder = feeder;
        EventTypes = eventTypes;

        if (EventTypes.Any(e => !typeof(IEvent).IsAssignableFrom(e)))
        {
            throw new ArgumentException("All event types must implement IEvent interface.", nameof(eventTypes));
        }
    }
#else
    public EventContextFactoryRegistry(IEventContextFeeder feeder, params Type[] eventTypes)
    {
        Feeder = feeder;
        EventTypes = eventTypes;

        if (EventTypes.Any(e => !typeof(IEvent).IsAssignableFrom(e)))
        {
            throw new ArgumentException("All event types must implement IEvent interface.", nameof(eventTypes));
        }
    }
#endif

    // Constructor for single event type
    public EventContextFactoryRegistry(IEventContextFeeder feeder, Type eventType)
    {
        Feeder = feeder;
        EventTypes = new[] { eventType };

        if (!typeof(IEvent).IsAssignableFrom(eventType))
        {
            throw new ArgumentException("Event type must implement IEvent interface.", nameof(eventType));
        }
    }

    // Constructor for no specific event types (handles all)
    public EventContextFactoryRegistry(IEventContextFeeder feeder)
    {
        Feeder = feeder;
        EventTypes = Enumerable.Empty<Type>();
    }
}
