namespace FrameBox.Core.Events.Interfaces;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset RaisedAt { get; }
}
