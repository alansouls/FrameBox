using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Events.Defaults;

/// <summary>
/// Base class for handlers that deal with more than one event type. Implement
/// <see cref="IEventHandler{TEvent}"/> for each handled event and this class routes the incoming
/// event to the matching typed <c>HandleAsync</c> overload.
/// </summary>
/// <remarks>
/// Use <see cref="EventHandler{TDomainEvent}"/> when a single event is handled - it cannot be
/// inherited more than once, which is why multi event handlers derive from this class instead.
/// </remarks>
public abstract class MultiEventHandler : IEventHandler
{
    private delegate Task EventDispatcher(object handler, IEvent @event, CancellationToken cancellationToken);

    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, EventDispatcher>> Dispatchers = new();

    public Task HandleAsync(IEvent @event, CancellationToken cancellationToken)
    {
        var handlerType = GetType();
        var dispatchers = Dispatchers.GetOrAdd(handlerType, BuildDispatchers);

        if (!dispatchers.TryGetValue(@event.GetType(), out var dispatcher))
        {
            throw new InvalidOperationException(
                $"Handler {handlerType.FullName} does not handle events of type {@event.GetType().FullName}.");
        }

        return dispatcher.Invoke(this, @event, cancellationToken);
    }

    private static IReadOnlyDictionary<Type, EventDispatcher> BuildDispatchers(Type handlerType)
    {
        var eventHandlerInterfaces = handlerType.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>))
            .ToArray();

        if (eventHandlerInterfaces.Length == 0)
        {
            throw new InvalidOperationException($"Type {handlerType.FullName} does not implement IEventHandler<T> interface.");
        }

        var dispatchers = new Dictionary<Type, EventDispatcher>();

        foreach (var eventHandlerInterface in eventHandlerInterfaces)
        {
            var eventType = eventHandlerInterface.GenericTypeArguments[0];

            var handleMethod = eventHandlerInterface.GetMethod(nameof(HandleAsync), [eventType, typeof(CancellationToken)])
                ?? throw new InvalidOperationException(
                    $"Could not find HandleAsync method on {eventHandlerInterface.FullName}.");

            dispatchers[eventType] = BuildDispatcher(eventHandlerInterface, eventType, handleMethod);
        }

        return dispatchers;
    }

    private static EventDispatcher BuildDispatcher(Type eventHandlerInterface, Type eventType, MethodInfo handleMethod)
    {
        var handlerParameter = Expression.Parameter(typeof(object), "handler");
        var eventParameter = Expression.Parameter(typeof(IEvent), "event");
        var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");

        var call = Expression.Call(
            Expression.Convert(handlerParameter, eventHandlerInterface),
            handleMethod,
            Expression.Convert(eventParameter, eventType),
            cancellationTokenParameter);

        return Expression.Lambda<EventDispatcher>(call, handlerParameter, eventParameter, cancellationTokenParameter)
            .Compile();
    }
}
