using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StockApi.Exceptions;

namespace StockApi.Middleware;

public class GlobalExceptionHandler
    (ILogger<GlobalExceptionHandler> logger, IHostEnvironment hostEnvironment)
    : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred: {exceptionMessage}", exception.Message);

        (string Detail, string Title, int StatusCode) details = exception switch
        {
            NotFoundException =>
            (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound
            ),
            _ =>
            (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError

            )
        };

        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Detail = details.Detail,
            Status = details.StatusCode,
            Instance = httpContext.Request.Path
        };
        if (hostEnvironment.IsDevelopment())
        {
            problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
        }

        problemDetails.Extensions.Add("traceId", httpContext.TraceIdentifier);
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}