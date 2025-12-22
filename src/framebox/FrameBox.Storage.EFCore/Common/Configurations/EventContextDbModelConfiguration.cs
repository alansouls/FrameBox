using FrameBox.Storage.EFCore.EventContexts.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrameBox.Storage.EFCore.Common.Configurations;

public class EventContextDbModelConfiguration : IEntityTypeConfiguration<EventContextDbModel>
{
    private readonly string _schemaName;
    private readonly string _tableName;
    private readonly bool _excludeFromMigrations;

    public EventContextDbModelConfiguration(string? schemaName = null, string? tableName = null, bool excludeFromMigrations = false)
    {
        _schemaName = schemaName ?? "FrameBox";
        _tableName = tableName ?? "EventContexts";
        _excludeFromMigrations = excludeFromMigrations;
    }

    public void Configure(EntityTypeBuilder<EventContextDbModel> builder)
    {
        builder.ToTable(_tableName, _schemaName, action =>
        {
            if (_excludeFromMigrations)
            {
                action.ExcludeFromMigrations();
            }
        });

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Type).IsRequired().HasMaxLength(200);

        builder.OwnsMany(e => e.LinkedEvents, ownedBuild =>
        {
            ownedBuild.ToTable($"{_tableName}_Links", _schemaName);

            ownedBuild.HasKey(le => new { le.EventContextId, le.EventId });

            ownedBuild.WithOwner().HasForeignKey(le => le.EventContextId);
        });

        builder.Property(b => b.DataJson).IsRequired();
    }
}
