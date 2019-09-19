using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.HeadElement;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.Extensions.DependencyInjection
{
    public static class HeadElementDependencyInjection
    {
        public static IServiceCollection AddHeadElementHelper(this IServiceCollection services)
        {
            services.AddScoped<IHeadElementHelperStore, HeadElementHelperStore>();
            services.AddScoped<HeadElementHelper, HeadElementHelperService>();
            return services;
        }
    }
}
