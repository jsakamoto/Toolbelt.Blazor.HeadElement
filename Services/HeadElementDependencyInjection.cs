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
#pragma warning disable CS0618 // Type or member is obsolete
            return services.AddHeadElementHelper(configure: null);
#pragma warning restore CS0618 // Type or member is obsolete
        }

#if ENABLE_JSMODULE
        [System.Obsolete("The \"DisableClientScriptAutoInjection\" option is no longer effective in .net 5.0.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
        public static IServiceCollection AddHeadElementHelper(this IServiceCollection services, Action<HeadElementHelperServiceOptions> configure)
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
