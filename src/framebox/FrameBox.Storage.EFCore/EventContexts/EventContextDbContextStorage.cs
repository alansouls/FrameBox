using System.Text.Json;
using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;
using FrameBox.Storage.EFCore.Common;
using FrameBox.Storage.EFCore.EventContexts.Models;
using Microsoft.EntityFrameworkCore;

namespace FrameBox.Storage.EFCore.EventContexts;

public class EventContextDbContextStorage : IEventContextStorage
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };
    
    private readonly InternalDbContextWrapper _dbContextWrapper;

    internal EventContextDbContextStorage(InternalDbContextWrapper dbContextWrapper)
    {
        _dbContextWrapper = dbContextWrapper;
    }

    public async Task AddAsync(IEnumerable<IEventContext> eventContexts, CancellationToken cancellationToken)
    {
        var models = eventContexts.Select(e => new EventContextDbModel()
        {
            Id = e.Id,
            LinkedEvents = e.LinkedEvents,
            DataJson = JsonSerializer.Serialize(e.Data, JsonSerializerOptions)
        }).ToList();
        
        await _dbContextWrapper.Context.AddRangeAsync(models, cancellationToken);
    }

    public async Task<IEnumerable<IEventContext>> GetEventContextsAsync(Guid eventId, CancellationToken cancellationToken)
    {
        return await _dbContextWrapper.Context.Set<EventContextDbModel>()
            .AsNoTracking()
            .Where(ec => ec.LinkedEvents.Contains(eventId))
            .Select(ec => new EventContext
            {
                Id = ec.Id,
                LinkedEvents = ec.LinkedEvents,
                Data = JsonSerializer.Deserialize<Dictionary<string, string>>(ec.DataJson, JsonSerializerOptions) ?? new Dictionary<string, string>()
            })
            .ToListAsync(cancellationToken);
    }
}