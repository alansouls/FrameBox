using FrameBox.Core.Inbox.Models;

namespace FrameBox.Core.Events.Interfaces;

public interface IEventHandlerProvider
{
    IEventHandler GetEventHandler(string handlerName);
    IEnumerable<Type> GetEventHandlerTypes(Type eventType);
}
