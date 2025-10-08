namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContext
{
    Guid Id { get; }
    string Type { get; }
    IEnumerable<Guid> LinkedEvents { get; }
    
    IReadOnlyDictionary<string, string> Data { get; }

    void Push(string key, string value);
}
