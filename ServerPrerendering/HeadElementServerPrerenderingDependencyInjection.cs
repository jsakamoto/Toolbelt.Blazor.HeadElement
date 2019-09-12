using Microsoft.AspNetCore.Builder;
using Toolbelt.Blazor.HeadElement.Middlewares;

namespace Toolbelt.Blazor.Extensions.DependencyInjection
{
    public static class HeadElementServerPrerenderingDependencyInjection
    {
        public static IApplicationBuilder UseHeadElementServerPrerendering(this IApplicationBuilder app)
        {
            app.UseMiddleware<HeadElementServerPrerenderingMiddleware>();
            return app;
        }
    }
}
