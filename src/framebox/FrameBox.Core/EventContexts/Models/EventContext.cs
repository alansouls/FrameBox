using FrameBox.Core.EventContexts.Interfaces;

namespace FrameBox.Core.EventContexts.Models;

public class EventContext : IEventContext
{
    private readonly Dictionary<string, string> _data = new();

    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Type { get; set; }
    public IEnumerable<Guid> LinkedEvents { get; set; } = [];

    public void Push(string key, string value)
    {
        _data.Add(key, value);
    }

    public IReadOnlyDictionary<string, string> Data
    {
        get => _data;
        init => _data = value.ToDictionary();
    }
}
