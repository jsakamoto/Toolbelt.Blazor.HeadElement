using SampleSite.Components.Services;
using SampleSite.Server;
using SampleSite.Server.Data;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var appOptions = new AppOptions();
builder.Configuration.Bind(appOptions);
builder.Services.AddSingleton(appOptions);

// Add services to the container.
builder.Services.AddHeadElementHelper(options =>
{
    options.DisableClientScriptAutoInjection = appOptions.DisableClientScriptAutoInjection;
});
builder.Services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHeadElementServerPrerendering();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

