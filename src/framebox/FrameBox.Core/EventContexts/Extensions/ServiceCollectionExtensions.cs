using FrameBox.Core.EventContexts.Builders;
using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Core.EventContexts.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventContextManager(this IServiceCollection services, Action<EventContextFeederRegistryBuilder> builderAction)
    {
        var builder = new EventContextFeederRegistryBuilder(services);

        builderAction(builder);

        services.TryAddScoped<IEventContextManager, EventContextManager>();

        return services;
    }
}
