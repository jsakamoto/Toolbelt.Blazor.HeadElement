using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using SeattleTouristAttractions.Client.Services;
using SeattleTouristAttractions.Shared;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SeattleTouristAttractions.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPlacesService, ClientSidePlaceService>();
            services.AddHeadElementHelper();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
