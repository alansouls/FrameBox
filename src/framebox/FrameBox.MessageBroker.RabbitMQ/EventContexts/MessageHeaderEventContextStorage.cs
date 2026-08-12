using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;
using FrameBox.MessageBroker.RabbitMQ.Services;

namespace FrameBox.MessageBroker.RabbitMQ.EventContexts;

public class MessageHeaderEventContextStorage : IEventContextStorage
{
    private readonly MessageHeaderHolder _messageHeaderHolder;
    private readonly Dictionary<Guid, List<IEventContext>> _contexts = [];

    public MessageHeaderEventContextStorage(MessageHeaderHolder messageHeaderHolder)
    {
        _messageHeaderHolder = messageHeaderHolder;
    }

    public Task AddAsync(IEnumerable<IEventContext> eventContexts, CancellationToken cancellationToken)
    {
        foreach (var context in eventContexts)
        {
            foreach (var eventId in context.LinkedEvents)
            {
                foreach (var data in context.Data)
                {
                    if (context.Type.Contains('-') || data.Key.Contains('-'))
                    {
                        throw new InvalidOperationException("Context.Type and data key cannot have '-' character");
                    }
                    _messageHeaderHolder.AddHeader(eventId, $"x-{context.Type}-{data.Key}", data.Value);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task<IEnumerable<IEventContext>> GetEventContextsAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var headers = _messageHeaderHolder.GetHeaders(eventId);

        List<IEventContext> result = [];

        foreach (var eventHeaders in headers.Where(h => h.Key.StartsWith("x-") && h.Key.Split('-').Length == 3).GroupBy(h => h.Key.Split('-')[1]))
        {
            var data = eventHeaders.ToDictionary(header => header.Key.Split('-')[2], header => header.Value);
            result.Add(new EventContext
            {
                Id = Guid.NewGuid(),
                Type = eventHeaders.Key,
                Data = data,
                LinkedEvents = [eventId]
            });
        }

        return Task.FromResult<IEnumerable<IEventContext>>(result);
    }
}
