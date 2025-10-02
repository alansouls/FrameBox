using Microsoft.EntityFrameworkCore;

namespace FrameBox.Storage.EFCore.Common;

/// <summary>
/// This class is used so we can strongly use dependency injection to use the desired DbContext.
/// </summary>
internal class InternalDbContextWrapper<T>(DbContext context)
{
    public DbContext Context => context;
}
