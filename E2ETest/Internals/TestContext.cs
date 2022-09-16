using HeadElement.E2ETest.Internals;
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
        {new SampleSiteKey(Wasm,       NETCore31), new SampleSite(5011, "Client31", "netstandard2.1") },
        {new SampleSiteKey(WasmHosted, NETCore31), new SampleSite(5012, "Host",   "netcoreapp3.1")},
        {new SampleSiteKey(Server,     NETCore31), new SampleSite(5013, "Server", "netcoreapp3.1")},

        {new SampleSiteKey(Wasm,          NET50),  new SampleSite(5014, "Client", "net5.0")},
        {new SampleSiteKey(WasmPublished, NET50),  new SampleSite(5015, "Client", "net5.0", published: true)},
        {new SampleSiteKey(WasmHosted,    NET50),  new SampleSite(5016, "Host",   "net5.0")},
        {new SampleSiteKey(Server,        NET50),  new SampleSite(5017, "Server", "net5.0")},

        {new SampleSiteKey(Wasm,          NET60),  new SampleSite(5018, "Client", "net6.0")},
        {new SampleSiteKey(WasmPublished, NET60),  new SampleSite(5019, "Client", "net6.0", published: true)},
        {new SampleSiteKey(WasmHosted,    NET60),  new SampleSite(5020, "Host",   "net6.0")},
        {new SampleSiteKey(Server,        NET60),  new SampleSite(5021, "Server", "net6.0")},
    };

    private IPlaywright? _Playwrite;

    private IBrowser? _Browser;

    private IPage? _Page;

    public async ValueTask<IPage> GetPageAsync()
    {
        this._Playwrite ??= await Playwright.CreateAsync();

        if (this._Browser == null)
        {
            var browserType = this._Playwrite.Chromium;
            //var browserType = this._Playwrite.Firefox;
            this._Browser = await browserType.LaunchAsync(new()
            {
                //Channel = "msedge",
                Headless = false,
            });
        }

        this._Page ??= await this._Browser.NewPageAsync();

        return this._Page;
    }

    public ValueTask<SampleSite> StartHostAsync(HostingModel hostingModel, BlazorVersion blazorVersion)
    {
        return SampleSites[new SampleSiteKey(hostingModel, blazorVersion)].StartAsync();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Default = this;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDownAsync()
    {
        if (this._Browser != null) await this._Browser.DisposeAsync();
        this._Playwrite?.Dispose();
        Parallel.ForEach(SampleSites.Values, sampleSite => sampleSite.Stop());
    }
}
