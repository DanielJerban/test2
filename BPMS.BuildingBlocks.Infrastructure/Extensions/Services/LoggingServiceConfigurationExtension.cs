using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace BPMS.BuildingBlocks.Infrastructure.Extensions.Services;

public static class LoggingServiceConfigurationExtension
{
    public static IServiceCollection AddApplicationLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration().
            ReadFrom.Configuration(configuration).
            CreateLogger();

        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        return services;
    }

    public static IApplicationBuilder UseApplicationLogging(this IApplicationBuilder app)
    {
        var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
        loggerFactory.AddSerilog();

        var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (IDiagnosticContext diagnosticContext, HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirst("sid")?.Value;
                if (!string.IsNullOrEmpty(userId))
                    diagnosticContext.Set("UserId", userId);
            };
        });

        return app;
    }
}