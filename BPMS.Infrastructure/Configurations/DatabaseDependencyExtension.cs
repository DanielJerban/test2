using BPMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Infrastructure.Configurations;

internal static class DatabaseDependencyExtension
{
    public static void AddDatabaseDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
    }

    private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var bpmsSqlConnectionString = configuration.GetConnectionString("BPMS-Sql");

        services.AddDbContext<BpmsDbContext>(c => c.UseSqlServer(bpmsSqlConnectionString));
    }
}