using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SampleSite.Client.Services;
using SampleSite.Components;
using SampleSite.Components.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SampleSite.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            ConfigureServices(builder.Services, builder.HostEnvironment);

            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
        {
            services
                .AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress) })
                .AddScoped<IWeatherForecastService, WeatherForecastService>()
                .AddHeadElementHelper(options =>
                {
                    // options.DisableClientScriptAutoInjection = true;
                });
        }
    }
}
