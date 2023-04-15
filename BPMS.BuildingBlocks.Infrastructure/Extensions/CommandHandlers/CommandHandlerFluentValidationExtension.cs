using System.Reflection;
using BPMS.BuildingBlocks.Infrastructure.Configuration.Processing;
using FluentValidation;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.BuildingBlocks.Infrastructure.Extensions.CommandHandlers;

public static class CommandHandlerFluentValidationExtension
{
    public static IServiceCollection AddCommandHandlerFluentValidation(this IServiceCollection services, IEnumerable<Assembly> assemblies,
        ServiceLifetime lifetime = ServiceLifetime.Transient, Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null)
    {
        services.Add(new ServiceDescriptor(typeof(IRequestPreProcessor<>), typeof(CommandHandlerFluentValidationPreProcessor<>), lifetime));

        services.AddValidatorsFromAssemblies(assemblies, lifetime, filter);

        return services;
    }
}