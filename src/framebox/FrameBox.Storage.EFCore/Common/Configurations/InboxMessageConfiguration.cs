using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrameBox.Storage.EFCore.Common.Configurations;

public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    private readonly string _schemaName;
    private readonly string _tableName;

    public InboxMessageConfiguration(string? schemaName, string? tableName = null)
    {
        _schemaName = schemaName ?? "FrameBox";
        _tableName = tableName ?? "InboxMessages";
    }

    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable(_tableName, _schemaName);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.EventId)
            .IsRequired();
        
        builder.Property(p => p.EventName)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(p => p.EventPayload)
            .IsRequired();

        builder.Property(p => p.HandlerName)
            .IsRequired();
        builder.Property(p => p.RetryCount)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
        builder.Property(p => p.State)
            .IsRequired();
        builder.Property(p => p.ProcessId);
        builder.Property(p => p.FailurePayload);

        builder.HasIndex(p => p.ProcessId);
        builder.HasIndex(p => p.State);
        
        builder.HasIndex(p => p.EventId);
    }
}
