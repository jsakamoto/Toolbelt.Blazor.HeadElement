using HeadElement.E2ETest.Internals;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
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

    private ChromeDriver? _ChromeDriver;
    private FirefoxDriver? _FirefoxDriver;

    public IWebDriver WebDriver
    {
        get
        {
            //if (this._WebDriver == null)
            //{
            //    this._WebDriver = new ChromeDriver();
            //    // this._WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            //}
            //return this._WebDriver;
            if (this._FirefoxDriver == null)
            {
                this._FirefoxDriver = new FirefoxDriver();
                // this._WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            }
            return this._FirefoxDriver;
        }
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
    public void OneTimeTearDown()
    {
        Parallel.ForEach(SampleSites.Values, sampleSite => sampleSite.Stop());
        this._ChromeDriver?.Quit();
        this._FirefoxDriver?.Quit();
    }
}
