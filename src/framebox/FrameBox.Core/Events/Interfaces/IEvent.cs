namespace FrameBox.Core.Events.Interfaces;

public interface IEvent
{
    Guid Id { get; }
    DateTimeOffset RaisedAt { get; }

    string GetName() => GetType().FullName ?? throw new InvalidDataException("Invalid event type");
}
