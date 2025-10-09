using InboxOutboxSample.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;
using FrameBox.Core.Events.Extensions;
using InboxOutboxSample.ApiService.Domain;

namespace InboxOutboxSample.Shared.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.RegisterEventsFromAssemblyContaining<PaymentCreatedEvent>();
        services.AddEventHandlersFromAssemblyContaining<PaymentCreatedHandler>();

        services.AddScoped<UsefulDataBag>();

        return services;
    }
}
