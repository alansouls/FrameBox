using FrameBox.Core.Events.Interfaces;

namespace FrameBox.Core.Events.Defaults;

/// <summary>
/// Base implementation for FrameBox IEvent
/// </summary>
public abstract record FrameEvent : IEvent
{
    public virtual Guid Id { get; init; } = Guid.NewGuid();

    public virtual DateTimeOffset RaisedAt { get; init; } = DateTimeOffset.UtcNow;
}
