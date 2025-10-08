using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.EventContexts.Services;

public class EventContextFactoryRegistry
{
    public IEventContextFeeder Feeder { get; }

    public IEnumerable<Type> EventTypes { get; }

    public EventContextFactoryRegistry(IEventContextFeeder feeder, params IEnumerable<Type> eventTypes)
    {
        Feeder = feeder;
        EventTypes = eventTypes;

        if (EventTypes.Any(e => !typeof(IEvent).IsAssignableFrom(e)))
        {
            throw new ArgumentException("All event types must implement IEvent interface.", nameof(eventTypes));
        }
    }
}
