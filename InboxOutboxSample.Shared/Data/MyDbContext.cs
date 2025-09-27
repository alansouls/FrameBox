using FrameBox.Storage.EFCore.Common.Extensions;
using InboxOutboxSample.ApiService.Domain;
using Microsoft.EntityFrameworkCore;

namespace InboxOutboxSample.Shared.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureOutbox();
        modelBuilder.ConfigureInbox();

        var paymentEntity = modelBuilder.Entity<Payment>();

        paymentEntity.ToTable("Payments");
        paymentEntity.HasKey(p => p.Id);
        paymentEntity.Property(p => p.Amount);
    }
}
