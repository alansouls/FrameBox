using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;

namespace FrameBox.Core.EventContexts.Services;

public sealed class DefaultEventContextStorageFactory : IEventContextStorageFactory
{
    private readonly IEventContextStorage _storage;

    public DefaultEventContextStorageFactory(IEventContextStorage storage)
    {
        _storage = storage;
    }

    public EventContextStorageHolder GetStorage() => new(_storage, scope: null);
}
