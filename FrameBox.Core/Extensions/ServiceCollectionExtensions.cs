using FrameBox.Core.Events.Extensions;
using FrameBox.Core.Inbox.Extensions;
using FrameBox.Core.Outbox.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameBoxCore(this IServiceCollection services)
    {
        services.AddOutboxServices();
        services.AddInboxServices();
        services.AddEventDefaultServices();

        return services;
    }
}
