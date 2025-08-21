using FrameBox.Core.Outbox.Defaults;
using FrameBox.Core.Outbox.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Core.Outbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IOutboxMessageFactory, DefaultOutboxMessageFactory>();
        services.TryAddSingleton<IOutboxDispatcher, OutboxDispatcher>();
        services.AddHostedService(sp => (OutboxDispatcher)sp.GetRequiredService<IOutboxDispatcher>());

        return services;
    }
}
