using FrameBox.Core.Events.Defaults;
using FrameBox.Core.Events.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.Core.Events.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventDefaultServices(this IServiceCollection services)
    {
        services.TryAddSingleton<IDomainEventSerializer, JsonDomainEventSerializer>();

        return services;
    }
}
