using System.Text.Json;
using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;
using FrameBox.Storage.EFCore.Common;
using FrameBox.Storage.EFCore.EventContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace FrameBox.Storage.EFCore.EventContexts;

internal class EventContextDbContextStorage : IEventContextStorage
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly InternalDbContextWrapper<IEventContextStorage> _dbContextWrapper;

    public EventContextDbContextStorage(InternalDbContextWrapper<IEventContextStorage> dbContextWrapper)
    {
        _dbContextWrapper = dbContextWrapper;
    }

    public async Task AddAsync(IEnumerable<IEventContext> eventContexts, CancellationToken cancellationToken)
    {
        var models = eventContexts.Select(e => new EventContextDbModel()
        {
            Id = e.Id,
            Type = e.Type,
            LinkedEvents = e.LinkedEvents.Select(eventId => new EventContextEventLink
            {
                EventContextId = e.Id,
                EventId = eventId
            }).ToList(),
            DataJson = JsonSerializer.Serialize(e.Data, JsonSerializerOptions)
        }).ToList();

        await _dbContextWrapper.Context.AddRangeAsync(models, cancellationToken);
        await _dbContextWrapper.Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<IEventContext>> GetEventContextsAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var eventContexts = await _dbContextWrapper.Context.Set<EventContextDbModel>()
            .AsNoTracking()
            .Where(ec => ec.LinkedEvents.Any(le => le.EventId == eventId))
            .ToListAsync(cancellationToken);

        return eventContexts.Select(ec => new EventContext
        {
            Id = ec.Id,
            Type = ec.Type,
            LinkedEvents = ec.LinkedEvents.Select(l => l.EventId),
            Data = JsonSerializer.Deserialize<Dictionary<string, string>>(ec.DataJson, JsonSerializerOptions) ?? new Dictionary<string, string>()
        }).ToList();
    }
}