using BPMS.Application.CQRS.Sample.Create;
using BPMS.BuildingBlocks.Infrastructure.Extensions.CommandHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application;

public static class ApiDependencies
{
    public static void AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediator();
    }
        
    private static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddCommandHandlerFluentValidation(new[] { typeof(CreateSampleCommandValidator).Assembly });
        services.AddCommandHandlerEntityEvent();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateSampleCommand).Assembly));

        return services;
    }
}