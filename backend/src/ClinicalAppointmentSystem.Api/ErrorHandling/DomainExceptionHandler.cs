using ClinicalAppointmentSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClinicalAppointmentSystem.Api.ErrorHandling;

internal sealed class DomainExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var (statusCode, title) = domainException switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            ConflictException => (StatusCodes.Status409Conflict, "Request conflicts with the current state"),
            DomainValidationException => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
            _ => (StatusCodes.Status400BadRequest, "Request could not be processed"),
        };

        logger.LogInformation(
            "Domain rule refused {Method} {Path}: {ErrorCode} — {Message}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            domainException.ErrorCode,
            domainException.Message);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = domainException.Message,
            Instance = httpContext.Request.Path,
        };

        problemDetails.Extensions["errorCode"] = domainException.ErrorCode;

        if (domainException is DomainValidationException { Errors.Count: > 0 } validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        foreach (var (key, value) in domainException.Extensions)
        {
            problemDetails.Extensions[key] = value;
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception,
        });
    }
}
