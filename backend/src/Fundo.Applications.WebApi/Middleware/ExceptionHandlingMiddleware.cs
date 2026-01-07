using System.Net;
using Fundo.Infrastructure.Common;

namespace Fundo.Applications.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var path = context.Request.Path;
        var method = context.Request.Method;

        var (statusCode, message, logLevel) = exception switch
        {
            ArgumentException => (
                (int)HttpStatusCode.BadRequest,
                exception.Message,
                LogLevel.Warning
            ),
            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                "Unauthorized access",
                LogLevel.Warning
            ),
            KeyNotFoundException => (
                (int)HttpStatusCode.NotFound,
                exception.Message,
                LogLevel.Warning
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "An internal server error occurred",
                LogLevel.Error
            )
        };

        // Structured logging with context
        _logger.Log(
            logLevel,
            exception,
            "Exception occurred while processing request. TraceId: {TraceId}, Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, ExceptionType: {ExceptionType}",
            traceId,
            method,
            path,
            statusCode,
            exception.GetType().Name
        );

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = message,
            StatusCode = statusCode,
            TraceId = traceId
        });
    }
}
