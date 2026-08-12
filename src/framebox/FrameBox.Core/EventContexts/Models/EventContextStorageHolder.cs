using FrameBox.Core.EventContexts.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.EventContexts.Models;

public sealed class EventContextStorageHolder : IDisposable
{
    private readonly IServiceScope? _scope;
    private readonly IEventContextStorage _storage;

    public EventContextStorageHolder(IEventContextStorage storage, IServiceScope? scope)
    {
        _storage = storage;
        _scope = scope;
    }

    public IEventContextStorage Storage => _storage;

    public void Dispose()
    {
        _scope?.Dispose();
    }
}
