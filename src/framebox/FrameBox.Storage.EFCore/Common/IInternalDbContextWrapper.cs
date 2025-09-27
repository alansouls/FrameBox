using Microsoft.EntityFrameworkCore;

namespace FrameBox.Storage.EFCore.Common;

/// <summary>
/// This class is used so we can strongly use dependency injection to use the desired DbContext for messages.
/// </summary>
internal class InternalDbContextWrapper(DbContext context)
{
    public DbContext Context => context;
}
