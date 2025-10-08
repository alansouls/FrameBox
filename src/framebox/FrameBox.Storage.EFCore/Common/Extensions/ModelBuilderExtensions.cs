using FrameBox.Storage.EFCore.Common.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FrameBox.Storage.EFCore.Common.Extensions;

public static class ModelBuilderExtensions
{
    public static void ConfigureOutbox(this ModelBuilder modelBuilder, string? tableName = null, string? schemaName = null)
    {
        var configuration = new OutboxMessageConfiguration(schemaName, tableName);

        modelBuilder.ApplyConfiguration(configuration);
    }

    public static void ConfigureInbox(this ModelBuilder modelBuilder, string? tableName = null, string? schemaName = null)
    {
        var configuration = new InboxMessageConfiguration(schemaName, tableName);

        modelBuilder.ApplyConfiguration(configuration);
    }

    public static void ConfigureEventContext(this ModelBuilder modelBuilder, string? tableName = null, string? schemaName = null)
    {
        var configuration = new EventContextDbModelConfiguration(schemaName, tableName);

        modelBuilder.ApplyConfiguration(configuration);
    }
}
