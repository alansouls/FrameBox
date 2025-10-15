using FrameBox.Core.EventContexts.Builders;
using FrameBox.Core.EventContexts.Extensions;
using FrameBox.Core.Events.Extensions;
using FrameBox.Core.Inbox.Extensions;
using FrameBox.Core.Inbox.Options;
using FrameBox.Core.Outbox.Extensions;
using FrameBox.Core.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameBoxCore(this IServiceCollection services,
        Action<EventContextFeederRegistryBuilder> configureEventContextFactoryRegistry,
        Action<InboxOptions>? configureInboxOptions = null,
        Action<OutboxOptions>? configureOutboxOptions = null)
    {
        services.AddOutboxServices();
        services.AddInboxServices();
        services.AddEventDefaultServices();
        services.AddEventContextManager(configureEventContextFactoryRegistry);

        var inboxOptions = new InboxOptions();

        if (configureInboxOptions is not null)
        {
            configureInboxOptions(inboxOptions);
        }

        InternalInboxOptions.ApplyOptions(inboxOptions);

        var outboxOptions = new OutboxOptions();

        if (configureOutboxOptions is not null)
        {
            configureOutboxOptions(outboxOptions);
        }

        InternalOutboxOptions.ApplyOptions(outboxOptions);

        return services;
    }
}
