using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Common.Entities;

public abstract class BaseEntity
{
    private readonly List<IEvent> _events = [];

    public IReadOnlyList<IEvent> Events => _events.AsReadOnly();

    public List<IEvent> ConsumeEvents()
    {
        var consumedEvents = _events.ToList();
        _events.Clear();

        return consumedEvents;
    }

    protected void RaiseEvent(IEvent domainEvent)
    {
        _events.Add(domainEvent);
    }
}
