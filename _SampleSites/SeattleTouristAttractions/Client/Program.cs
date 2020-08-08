using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SeattleTouristAttractions.Client.Services;
using SeattleTouristAttractions.Components;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SeattleTouristAttractions.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
                .AddScoped<IPlacesService, ClientSidePlaceService>()
                .AddHeadElementHelper();

            await builder.Build().RunAsync();
        }
    }
}
