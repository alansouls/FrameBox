using FrameBox.Core.Inbox.Defaults;
using FrameBox.Core.Inbox.Interfaces;
using FrameBox.Core.Outbox.Defaults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Core.Inbox.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInboxServices(this IServiceCollection services)
    {
        services.TryAddScoped<IInboxHandler, DefaultInboxHandler>();
        services.TryAddScoped<IInboxMessageFactory, DefaultInboxMessageFactory>();
        services.TryAddSingleton<IInboxDispatcher, InboxDispatcher>();
        services.AddHostedService(sp => (InboxDispatcher)sp.GetRequiredService<IInboxDispatcher>());
        services.AddHostedService<InboxTimeoutService>();

        return services;
    }
}
