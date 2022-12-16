using HeadElement.E2ETest.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using static HeadElement.E2ETest.Internals.BlazorVersion;
using static HeadElement.E2ETest.Internals.HostingModel;

namespace HeadElement.E2ETest;

[SetUpFixture]
public class TestContext
{
    public static TestContext? Default { get; private set; }

    public static readonly IReadOnlyDictionary<SampleSiteKey, SampleSite> SampleSites = new Dictionary<SampleSiteKey, SampleSite> {
        {new SampleSiteKey(Wasm,          NET60),  new SampleSite(5018, "Client", "net6.0")},
        {new SampleSiteKey(WasmPublished, NET60),  new SampleSite(5019, "Client", "net6.0", published: true)},
        {new SampleSiteKey(WasmHosted,    NET60),  new SampleSite(5020, "Host",   "net6.0")},
        {new SampleSiteKey(Server,        NET60),  new SampleSite(5021, "Server", "net6.0")},
        {new SampleSiteKey(Server,        NET60,   DisableScriptInjection: true),  new SampleSite(5022, "Server", "net6.0", disableScriptInjection: true)},

        {new SampleSiteKey(Wasm,          NET70),  new SampleSite(5030, "Client", "net7.0")},
        {new SampleSiteKey(WasmPublished, NET70),  new SampleSite(5031, "Client", "net7.0", published: true)},
        {new SampleSiteKey(WasmHosted,    NET70),  new SampleSite(5032, "Host",   "net7.0")},
        {new SampleSiteKey(Server,        NET70),  new SampleSite(5033, "Server", "net7.0")},
        {new SampleSiteKey(Server,        NET70,   DisableScriptInjection: true),  new SampleSite(5034, "Server", "net7.0", disableScriptInjection: true)},
    };

    private IPlaywright? _Playwrite;

    private IBrowser? _Browser;

    private IPage? _Page;

    private class TestOptions
    {
        public string Browser { get; set; } = "";

        public bool Headless { get; set; } = true;

        public bool SkipInstallBrowser { get; set; } = false;
    }

    private readonly TestOptions _Options = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "DOTNET_")
            .AddTestParameters()
            .Build();
        configuration.Bind(this._Options);

        Default = this;

        if (!this._Options.SkipInstallBrowser)
        {
            Microsoft.Playwright.Program.Main(new[] { "install" });
        }
    }

    public ValueTask<SampleSite> StartHostAsync(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        return SampleSites[new SampleSiteKey(hostingModel, blazorVersion, disableScriptInjection)].StartAsync();
    }

    public async ValueTask<IPage> GetPageAsync()
    {
        this._Playwrite ??= await Playwright.CreateAsync();
        this._Browser ??= await this.LaunchBrowserAsync(this._Playwrite);
        this._Page ??= await this._Browser.NewPageAsync();
        return this._Page;
    }

    private Task<IBrowser> LaunchBrowserAsync(IPlaywright playwright)
    {
        var browserType = this._Options.Browser.ToLower() switch
        {
            "firefox" => playwright.Firefox,
            "webkit" => playwright.Webkit,
            _ => playwright.Chromium
        };

        var channel = this._Options.Browser.ToLower() switch
        {
            "firefox" or "webkit" => "",
            _ => this._Options.Browser.ToLower()
        };

        return browserType.LaunchAsync(new()
        {
            Channel = channel,
            Headless = this._Options.Headless,
        });
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        if (this._Browser != null) await this._Browser.DisposeAsync();
        this._Playwrite?.Dispose();
        Parallel.ForEach(SampleSites.Values, sampleSite => sampleSite.Stop());
    }
}
