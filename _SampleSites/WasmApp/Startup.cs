using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace WasmApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHeadElementHelper();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
