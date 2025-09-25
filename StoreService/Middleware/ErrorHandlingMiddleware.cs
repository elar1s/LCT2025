using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace StoreService.Middleware;

/// <summary>
/// Global error handling middleware capturing unhandled exceptions and returning structured JSON errors.
/// </summary>
public class ErrorHandlingMiddleware
{
    #region Fields
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    #endregion

    #region Ctor
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware > logger)
    {
        _next = next;
        _logger = logger;
    }
    #endregion

    #region Invoke
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
    #endregion

    #region Helpers
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var status = ex switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var errorId = Guid.NewGuid().ToString();
        if (status >= 500)
        {
            _logger.LogError(ex, "Unhandled server exception {ErrorId}", errorId);
        }
        else
        {
            _logger.LogWarning(ex, "Handled domain exception {ErrorId} -> {Status}", errorId, status);
        }

        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();

        var problem = new
        {
            traceId = context.TraceIdentifier,
            errorId,
            status,
            message = ex.Message,
            details = env.IsDevelopment() ? ex.StackTrace : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
    #endregion
}


