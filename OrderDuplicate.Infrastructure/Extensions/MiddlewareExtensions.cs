using Microsoft.AspNetCore.Builder;

namespace OrderDuplicate.Infrastructure.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}




