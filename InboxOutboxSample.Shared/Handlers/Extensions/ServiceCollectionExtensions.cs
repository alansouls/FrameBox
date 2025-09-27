using FrameBox.Core.Events.Interfaces;
using InboxOutboxSample.ApiService.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace InboxOutboxSample.Shared.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<PaymentCreatedHandler>()
            .AddScoped<IEventHandler<PaymentCreatedEvent>, PaymentCreatedHandler>(provider => provider.GetRequiredService<PaymentCreatedHandler>());

        serviceCollection
            .AddScoped<LongRunningHandler>()
            .AddScoped<IEventHandler<PaymentCreatedEvent>, LongRunningHandler>(provider => provider.GetRequiredService<LongRunningHandler>());

        return serviceCollection;
    }
}
