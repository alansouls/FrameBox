using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.EventContexts.Services;

public sealed class EventContextStorageNewScopeFactory : IEventContextStorageFactory
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventContextStorageNewScopeFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public EventContextStorageHolder GetStorage()
    {
        var scope = _scopeFactory.CreateScope();
        return new EventContextStorageHolder(scope.ServiceProvider.GetRequiredService<IEventContextStorage>(), scope);
    }
}
