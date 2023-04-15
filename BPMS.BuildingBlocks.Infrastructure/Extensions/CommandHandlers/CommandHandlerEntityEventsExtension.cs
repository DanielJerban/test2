using BPMS.BuildingBlocks.Infrastructure.Configuration.Processing;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.BuildingBlocks.Infrastructure.Extensions.CommandHandlers;

public static class CommandHandlerEntityEventsExtension
{
    public static IServiceCollection AddCommandHandlerEntityEvent(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        services.Add(new ServiceDescriptor(typeof(IRequestPostProcessor<,>), typeof(CommandHandlerEntityEventPostProssesor<,>), lifetime));
        return services;
    }
}