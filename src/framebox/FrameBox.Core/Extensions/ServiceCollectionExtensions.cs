using FrameBox.Core.EventContexts.Builders;
using FrameBox.Core.EventContexts.Extensions;
using FrameBox.Core.Events.Extensions;
using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Inbox.Extensions;
using FrameBox.Core.Outbox.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameBoxCore(this IServiceCollection services, Action<EventContextFeederRegistryBuilder> configureEventContextFactoryRegistry)
    {
        services.AddOutboxServices();
        services.AddInboxServices();
        services.AddEventDefaultServices();
        services.AddEventContextManager(configureEventContextFactoryRegistry);

        return services;
    }
}
