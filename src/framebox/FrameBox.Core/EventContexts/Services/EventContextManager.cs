using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;
using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.EventContexts.Services;

public class EventContextManager : IEventContextManager
{
    private Dictionary<IEventContextFeeder, List<Type>>? _factoryEventTypes;

    private readonly IEnumerable<EventContextFactoryRegistry> _factoryRegistries;
    private readonly IServiceProvider _serviceProvider;

    public EventContextManager(IEnumerable<EventContextFactoryRegistry> factoryRegistries, IServiceProvider serviceProvider)
    {
        _factoryRegistries = factoryRegistries;
        _serviceProvider = serviceProvider;
    }

    public async Task CaptureContextsAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken)
    {
        var eventList = events.ToList();

        if (eventList.Count == 0)
        {
            return;
        }

        _factoryEventTypes ??= _factoryRegistries
            .GroupBy(r => r.Feeder)
            .ToDictionary(g => g.Key, g => g.Any(r => !r.EventTypes.Any()) ? [] : g.SelectMany(r => r.EventTypes).Distinct().ToList());

        List<IEventContext> contextsToSave = [];

        foreach (var (feeder, eventTypes) in _factoryEventTypes)
        {
            var relevantEventIds = eventTypes.Count == 0
                ? eventList.Select(e => e.Id).ToList()
                : eventList.Where(e => eventTypes.Any(t => t.IsInstanceOfType(e)))
                .Select(e => e.Id).ToList();

            if (relevantEventIds.Count == 0)
            {
                continue;
            }

            var context = new EventContext();

            await feeder.FeedAsync(context, cancellationToken);

            context.LinkedEvents = relevantEventIds;

            contextsToSave.Add(context);
        }

        var storage = _serviceProvider.GetRequiredService<IEventContextStorage>();

        await storage.AddAsync(contextsToSave, cancellationToken);
    }
}
