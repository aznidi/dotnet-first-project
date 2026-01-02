using FIRST.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FIRST.Extensions;

public static class ApiPipelineExtensions
{
    public static IApplicationBuilder UseApiStatusCodePages(this IApplicationBuilder app)
    {
        return app.UseStatusCodePages(async context =>
        {
            var res = context.HttpContext.Response;

            if (res.HasStarted) return;

            if (res.StatusCode == StatusCodes.Status404NotFound)
            {
                res.ContentType = "application/json";
                await res.WriteAsJsonAsync(ApiResponse<object>.Fail("Endpoint not found"));
            }
        });
    }
}
