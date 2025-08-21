namespace FrameBox.Core.Events.Interfaces;

public interface IDomainEvent
{
    Guid Id { get; }
    string EventName { get; }
    DateTimeOffset RaisedAt { get; }
}
