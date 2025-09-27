using FrameBox.Core.Events.Defaults;
using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FrameBox.Core.Events.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventDefaultServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();
        services.TryAddScoped<IEventHandlerProvider, DefaultEventHandlerProvider>();

        return services;
    }
}
