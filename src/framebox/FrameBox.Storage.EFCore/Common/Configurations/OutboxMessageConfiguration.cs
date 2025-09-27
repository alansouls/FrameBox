using FrameBox.Core.Outbox.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.Storage.EFCore.Common.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    private readonly string _schemaName;
    private readonly string _tableName;

    public OutboxMessageConfiguration(string? schemaName, string? tableName = null)
    {
        _schemaName = schemaName ?? "FrameBox";
        _tableName = tableName ?? "OutboxMessages";
    }

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(_tableName, _schemaName);

        builder.Ignore(p => p.Id);

        builder.HasKey(p => p.EventId);

        builder.Property(p => p.EventType)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(p => p.Payload)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
        builder.Property(p => p.State)
            .IsRequired();
        builder.Property(p => p.ProcessId);

        builder.HasIndex(p => p.ProcessId);
        builder.HasIndex(p => p.State);
    }
}
