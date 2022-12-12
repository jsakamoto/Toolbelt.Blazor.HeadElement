using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SampleSite.Client.Services;
using SampleSite.Components;
using SampleSite.Components.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("app");

ConfigureServices(builder.Services, builder.HostEnvironment);

await builder.Build().RunAsync();

static void ConfigureServices(IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
{
    services
        .AddScoped(sp => new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress) })
        .AddScoped<IWeatherForecastService, WeatherForecastService>()
        .AddHeadElementHelper(options =>
        {
            // options.DisableClientScriptAutoInjection = true;
        });
}
