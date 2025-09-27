using FrameBox.Core.Events.Interfaces;
using FrameBox.Core.Extensions;
using InboxOutboxSample.ApiService.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace InboxOutboxSample.Shared.Handlers.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddEventHandlersFromAssemblyContaining<PaymentCreatedHandler>();

        return serviceCollection;
    }
}
