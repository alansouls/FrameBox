using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Storage.EFCore.Common.Interceptors;
using FrameBox.Storage.EFCore.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Storage.EFCore.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxEntityFrameworkCoreStorage<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.TryAddScoped(serviceProvider => new InternalDbContextWrapper(serviceProvider.GetRequiredService<TDbContext>()));
        services.AddScoped<OutboxMessagesInterceptors>();
        services.AddScoped<IOutboxStorage, OutboxDbContextStorage>();

        return services;
    }
    public static IServiceCollection AddInboxEntityFrameworkCoreStorage<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.TryAddScoped(serviceProvider => new InternalDbContextWrapper(serviceProvider.GetRequiredService<TDbContext>()));
        services.AddScoped<IInboxStorage, InboxDbContextStorage>();

        return services;
    }
}
