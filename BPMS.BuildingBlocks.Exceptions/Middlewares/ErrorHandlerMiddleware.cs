using BPMS.BuildingBlocks.Domain;
using BPMS.BuildingBlocks.Domain.BusinessRule;
using BPMS.BuildingBlocks.Exceptions.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BPMS.BuildingBlocks.Exceptions.Middlewares;

public class ErrorHandlerMiddleware
{
    internal const string DefaultMessage = "The HyperText Transfer Protocol (HTTP) 500 Internal Server Error server error response code indicates that the server encountered an unexpected condition that prevented it from fulfilling the request";

    private readonly ILogger<ErrorHandlerMiddleware> logger;
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment environment;

    public ErrorHandlerMiddleware(RequestDelegate next,
        ILoggerFactory loggerFactory,
        IHostEnvironment environment)
    {
        logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
        _next = next;
        this.environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            logger.LogError(error, error.Message);
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = error switch
            {
                IBaseException exception => exception.StatusCode,
                IEntityNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status406NotAcceptable,
                TimeoutException => StatusCodes.Status408RequestTimeout,
                OperationCanceledException => StatusCodes.Status409Conflict,
                ArgumentException => StatusCodes.Status412PreconditionFailed,
                IDomainEventNullException => StatusCodes.Status417ExpectationFailed,
                BusinessRuleValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => 500,//internal server error
            };

            var message = error.InnerException?.Message ?? error.Message;
            if (environment.IsDevelopment())
            {
                if (error is NullReferenceException)
                    message = JsonConvert.SerializeObject(error);
            }
            else
            {
                if (response.StatusCode == StatusCodes.Status500InternalServerError || response.StatusCode == StatusCodes.Status424FailedDependency)
                    message = "خطای سرور رخ داده است!";
            }

            var result = JsonConvert.SerializeObject(new ErrorMessage { message = message });
            await response.WriteAsync(result);
        }
    }
}
internal class ErrorMessage
{
    public string message { get; set; } = ErrorHandlerMiddleware.DefaultMessage;
}