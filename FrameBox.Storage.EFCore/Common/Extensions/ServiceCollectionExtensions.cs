using FrameBox.Core.Outbox.Interfaces;
using FrameBox.Storage.EFCore.Common.Interceptors;
using FrameBox.Storage.EFCore.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.Storage.EFCore.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxEntityFrameworkCoreStorage<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped(serviceProvider => new InternalDbContextWrapper(serviceProvider.GetRequiredService<TDbContext>()));
        services.AddSingleton<OutboxMessagesInterceptors>();
        services.AddScoped<IOutboxStorage, OutboxDbContextStorage>();

        return services;
    }
}
