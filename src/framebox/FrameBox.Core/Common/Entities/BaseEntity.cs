using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Common.Entities;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();

    public List<IDomainEvent> ConsumeEvents()
    {
        var consumedEvents = _events.ToList();
        _events.Clear();

        return consumedEvents;
    }

    protected void RaiseEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }
}
