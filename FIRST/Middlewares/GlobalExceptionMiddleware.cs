using FIRST.Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace FIRST.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message)
            );
            return;
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                ApiResponse<object>.Fail(ex.Message)
            );
            return;
        }
        catch (Exception ex)
        {

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsJsonAsync(
                // ApiResponse<object>.Fail("Internal server error")
                ApiResponse<object>.Fail("Internal server error" + ex.Message)
            );
            _logger.LogError(ex, "An unexpected generic error occurred.");
        }
    }
}
