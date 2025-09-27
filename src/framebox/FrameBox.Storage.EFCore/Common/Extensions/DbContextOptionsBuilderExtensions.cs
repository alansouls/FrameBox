using FrameBox.Storage.EFCore.Common.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FrameBox.Storage.EFCore.Common.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseAsOutboxStorage(this DbContextOptionsBuilder optionsBuilder, 
        IServiceProvider serviceProvider)
    {
        optionsBuilder.AddInterceptors(
            serviceProvider.GetRequiredService<OutboxMessagesInterceptors>()
        );

        return optionsBuilder;
    }
}
