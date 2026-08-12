using FrameBox.Core.EventContexts.Models;

namespace FrameBox.Core.EventContexts.Interfaces;

public interface IEventContextStorageFactory
{
    EventContextStorageHolder GetStorage();
}
