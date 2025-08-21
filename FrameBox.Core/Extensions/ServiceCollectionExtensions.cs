using FrameBox.Core.Events.Extensions;
using FrameBox.Core.Outbox.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameBox.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFrameBoxCore(this IServiceCollection services)
    {
        services.AddOutboxServices();
        services.AddEventDefaultServices();

        return services;
    }
}
