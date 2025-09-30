namespace FrameBox.Core.Events.Interfaces;

public interface IEvent
{
    Guid Id { get; }
    DateTimeOffset RaisedAt { get; }
}
