using System.Collections.Generic;
using OpenQA.Selenium;
using Xunit;

namespace HeadElement.E2ETest
{
    [Collection(nameof(TestContext))]
    public class HeadElementOnBrowserTest
    {
        private readonly TestContext _TestContext;

        public HeadElementOnBrowserTest(TestContext testContext)
        {
            this._TestContext = testContext;
        }

        public static IEnumerable<object[]> HostingModels =>
        new List<object[]>
        {
            new object[] { HostingModel.Wasm },
            new object[] { HostingModel.WasmHosted },
            new object[] { HostingModel.Server },
        };

        [Theory(DisplayName = "Change Title on Browser (from Home)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeTitle_on_Browser_Start_from_Home_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to Home
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel));

            // Validate document title of "Home"
            driver.Wait(1000).Until(_ => driver.Title == "Sample Site");

            // Navigate to "Counter"
            driver.ClickCounter();

            // Validate document title of "Counter"
            driver.Wait(1000).Until(_ => driver.Title == "Counter(0)");

            // document title will be updated real time.
            driver.FindElement(By.CssSelector("button.btn-primary")).Click();
            driver.Wait(1000).Until(_ => driver.Title == "Counter(1)");
            driver.FindElement(By.CssSelector("button.btn-primary")).Click();
            driver.Wait(1000).Until(_ => driver.Title == "Counter(2)");

            // Navigate to "Fetch data"
            driver.ClickFetchData();

            // Validate document title of "Fetch data"
            driver.Wait(1000).Until(_ => driver.Title == "Fetch data");

            // Go back to "Home"
            driver.ClickHome();

            // Validate document title of "Home" was restored.
            driver.Wait(1000).Until(_ => driver.Title == "Sample Site");
        }

        [Theory(DisplayName = "Change Title on Browser (from Counter)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeTitle_on_Browser_Start_from_Counter_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to "Counter"
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel), "/counter");

            // Validate document title of "Counter"
            driver.Wait(1000).Until(_ => driver.Title == "Counter(0)");

            // document title will be updated real time.
            driver.FindElement(By.CssSelector("button.btn-primary")).Click();
            driver.Wait(1000).Until(_ => driver.Title == "Counter(1)");
            driver.FindElement(By.CssSelector("button.btn-primary")).Click();
            driver.Wait(1000).Until(_ => driver.Title == "Counter(2)");

            // Go back to "Home"
            driver.ClickHome();

            // Validate document title of "Home" was restored.
            driver.Wait(1000).Until(_ => driver.Title == "Sample Site");
        }


        [Theory(DisplayName = "Change meta elements on Browser (from Home)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeMetaElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to Home
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel));

            // Validate meta elements of "Home"
            driver.DumpMetaElements().Is(
                "'','','',''",
                "'meta-N0','','','value-N0-A'",
                "'meta-N2','','','value-N2-A'",
                "'meta-N3','','','value-N3-A'",
                "'viewport','','','width=device-width'",
                "'','meta-P0','','value-P0-A'",
                "'','meta-P2','','value-P2-A'",
                "'','meta-P3','','value-P3-A'",
                "'','','meta-H0','value-H0-A'",
                "'','','meta-H2','value-H2-A'",
                "'','','meta-H3','value-H3-A'");

            // Navigate to "Counter"
            driver.ClickCounter();

            // Validate meta elements of "Counter"
            driver.DumpMetaElements().Is(
                "'','','',''",
                "'meta-N0','','','value-N0-A'",
                "'meta-N1','','','value-N1-B'",
                "'meta-N2','','','value-N2-B'",
                "'viewport','','','width=device-width'",
                "'','meta-P0','','value-P0-A'",
                "'','meta-P1','','value-P1-B'",
                "'','meta-P2','','value-P2-B'",
                "'','','meta-H0','value-H0-A'",
                "'','','meta-H1','value-H1-B'",
                "'','','meta-H2','value-H2-B'");

            // Navigate to "Fetch data"
            driver.ClickFetchData();

            // Validate meta elements of "Fetch data"
            driver.DumpMetaElements().Is(
                "'','','',''",
                "'meta-N0','','','value-N0-A'",
                "'meta-N1','','','value-N1-C'",
                "'meta-N3','','','value-N3-A'",
                "'meta-N4','','','value-N4-C'",
                "'viewport','','','width=device-width'",
                "'','meta-P0','','value-P0-A'",
                "'','meta-P1','','value-P1-C'",
                "'','meta-P3','','value-P3-A'",
                "'','meta-P4','','value-P4-C'",
                "'','','meta-H0','value-H0-A'",
                "'','','meta-H1','value-H1-C'",
                "'','','meta-H3','value-H3-A'",
                "'','','meta-H4','value-H4-C'");

            // Go back to "Home"
            driver.ClickHome();

            // Validate meta elements of "Home" were restored.
            driver.DumpMetaElements().Is(
                "'','','',''",
                "'meta-N0','','','value-N0-A'",
                "'meta-N2','','','value-N2-A'",
                "'meta-N3','','','value-N3-A'",
                "'viewport','','','width=device-width'",
                "'','meta-P0','','value-P0-A'",
                "'','meta-P2','','value-P2-A'",
                "'','meta-P3','','value-P3-A'",
                "'','','meta-H0','value-H0-A'",
                "'','','meta-H2','value-H2-A'",
                "'','','meta-H3','value-H3-A'");
        }

        [Theory(DisplayName = "Change meta elements on Browser (from Counter)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeMetaElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to Home
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel), "/counter");

            // Validate meta elements of "Counter"
            var dump = driver.DumpMetaElements();
            driver.DumpMetaElements().Is(
                "'','','',''",
                "'meta-N0','','','value-N0-A'",
                "'meta-N1','','','value-N1-B'",
                "'meta-N2','','','value-N2-B'",
                "'viewport','','','width=device-width'",
                "'','meta-P0','','value-P0-A'",
                "'','meta-P1','','value-P1-B'",
                "'','meta-P2','','value-P2-B'",
                "'','','meta-H0','value-H0-A'",
                "'','','meta-H1','value-H1-B'",
                "'','','meta-H2','value-H2-B'");

            // Go back to "Home"
            driver.ClickHome();

            // Validate meta elements of "Home" were restored.
            driver.DumpMetaElements().Is(
                "'','','',''",
                "'meta-N0','','','value-N0-A'",
                "'meta-N2','','','value-N2-A'",
                "'meta-N3','','','value-N3-A'",
                "'viewport','','','width=device-width'",
                "'','meta-P0','','value-P0-A'",
                "'','meta-P2','','value-P2-A'",
                "'','meta-P3','','value-P3-A'",
                "'','','meta-H0','value-H0-A'",
                "'','','meta-H2','value-H2-A'",
                "'','','meta-H3','value-H3-A'");
        }
    }
}
