namespace FrameBox.Core.Events.Interfaces;

public interface IEventRegistry
{
    Type? GetEventType(string eventName);

    Type? GetEventHandlerType(string handlerName);

    string GetEventName(Type eventType);

    string GetHandlerName(Type handlerType);
}
