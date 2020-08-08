using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace HeadElement.E2ETest
{
    public class TestContext : IDisposable
    {
        private readonly IReadOnlyDictionary<HostingModel, SampleSite> SampleSites = new Dictionary<HostingModel, SampleSite> {
            { HostingModel.Wasm, new SampleSite(5011, "Client") },
            { HostingModel.WasmHosted, new SampleSite(5012, "Host") },
            { HostingModel.Server, new SampleSite(5013, "Server") }
        };

        private ChromeDriver _WebDriver;

        public ChromeDriver WebDriver
        {
            get
            {
                if (_WebDriver == null)
                {
                    _WebDriver = new ChromeDriver();
                    _WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
                }
                return _WebDriver;
            }
        }

        public TestContext()
        {
        }

        public void StartHost(HostingModel hostingModel)
        {
            this.SampleSites[hostingModel].Start();
        }

        public string GetHostUrl(HostingModel hostingModel)
        {
            return this.SampleSites[hostingModel].GetUrl();
        }

        public void Dispose()
        {
            Parallel.ForEach(SampleSites.Values, sampleSite => sampleSite.Stop());
            _WebDriver?.Quit();
        }
    }

    [CollectionDefinition(nameof(TestContext))]
    public class TestContextDefinition : ICollectionFixture<TestContext>
    {
    }
}
