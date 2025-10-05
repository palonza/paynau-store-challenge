
namespace Paynau.Api.Middleware;

using Paynau.Domain.Exceptions;
using System.Net;
using System.Text.Json;

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
        var statusCode = exception switch
        {
            ProductNotFoundException => HttpStatusCode.NotFound,
            InsufficientStockException => HttpStatusCode.BadRequest,
            InvalidQuantityException => HttpStatusCode.BadRequest,
            DuplicateProductException => HttpStatusCode.Conflict,
            ProductInUseException => HttpStatusCode.Conflict,
            ConcurrencyException => HttpStatusCode.Conflict,
            DomainException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };

        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = exception.Message,
            statusCode = (int)statusCode,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

