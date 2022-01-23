using System;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.HeadElement;
using Toolbelt.Blazor.HeadElement.Internals;

namespace Toolbelt.Blazor.Extensions.DependencyInjection
{
    public static class HeadElementDependencyInjection
    {
        public static IServiceCollection AddHeadElementHelper(this IServiceCollection services)
        {
            return services.AddHeadElementHelper(configure: null);
        }

        public static IServiceCollection AddHeadElementHelper(this IServiceCollection services, Action<HeadElementHelperServiceOptions>? configure)
        {
            services.AddScoped<IHeadElementHelperStore, HeadElementHelperStore>();
            services.AddScoped<IHeadElementHelper, HeadElementHelperService>(serviceProvider =>
            {
                var headElementHelper = new HeadElementHelperService(serviceProvider);
                configure?.Invoke(headElementHelper.Options);
                return headElementHelper;
            });
            return services;
        }
    }
}
