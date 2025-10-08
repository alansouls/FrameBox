using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Extensions;
using InboxOutboxSample.ApiService.Domain;
using InboxOutboxSample.Shared.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace InboxOutboxSample.Shared.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddEventHandlersFromAssemblyContaining<PaymentCreatedHandler>();

        services.AddScoped<UsefulDataBag>();

        return services;
    }
}
