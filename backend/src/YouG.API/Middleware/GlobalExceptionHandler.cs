using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace YouG.API.Middleware;

/// <summary>
/// Maps unhandled exceptions to RFC 7807 ProblemDetails (docs/04-API-DESIGN.md Section 1.5).
/// FluentValidation failures (from the MediatR ValidationBehavior) become 400s with per-field errors;
/// everything else becomes a generic 500 that doesn't leak internal details to the client.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                new ValidationProblemDetails(ToErrorDictionary(validationException))
                {
                    Type = "https://youg.app/errors/validation-failed",
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest
                } as ProblemDetails),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Type = "https://youg.app/errors/unexpected-error",
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError
                })
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static Dictionary<string, string[]> ToErrorDictionary(ValidationException exception) =>
        exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
}
