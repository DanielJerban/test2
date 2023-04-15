using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Infrastructure.Configurations;

public static class DependencyExtension
{
    public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseDependencies(configuration);
    }
}