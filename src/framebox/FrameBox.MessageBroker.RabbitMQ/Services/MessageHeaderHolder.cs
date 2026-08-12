namespace FrameBox.MessageBroker.RabbitMQ.Services;

public class MessageHeaderHolder
{
    private readonly Dictionary<Guid, Dictionary<string, string>> _headers = [];

    public void AddHeader(Guid eventId, string key, string value)
    {
        if (!_headers.TryGetValue(eventId, out var eventHeaders))
        {
            _headers[eventId] = eventHeaders = [];
        }

        eventHeaders[key] = value;
    }

    public IReadOnlyDictionary<string, string> GetHeaders(Guid eventId)
    {
        return _headers.GetValueOrDefault(eventId, []);
    }
}
