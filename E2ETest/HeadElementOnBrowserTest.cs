using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public static IEnumerable<object[]> HostingModels { get; } = new List<object[]>
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

            // Navigate to "Counter"
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel), "/counter");

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


        [Theory(DisplayName = "Change link elements on Browser (from Home)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeLinkElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to "Home", and validate link elements of "Home"
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel));
            driver.DumpLinkElements().Is(
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );

            // Navigate to "Counter", and validate link elements of "Counter"
            driver.ClickCounter();
            driver.DumpLinkElements().Is(
                "rel:canonical, href:/counter, type:, media:, title:link-B, sizes:",
                "rel:icon, href:/_content/SampleSite.Components/favicons/counter-0.png, type:image/png, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );

            // Navigate to "Fetch data", and validate link elements of "Fetch data"
            driver.ClickFetchData();
            driver.DumpLinkElements().Is(
                "rel:canonical, href:/fetchdata, type:, media:, title:link-C, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-C.css, type:, media:print, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );

            // Go back to "Home", and validate link elements of "Home" were restored.
            driver.ClickHome();
            driver.DumpLinkElements().Is(
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );
        }

        [Theory(DisplayName = "Change link elements on Browser (from Counter)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeLinkElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to "Counter", and validate link elements of "Counter"
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel), "/counter");
            driver.DumpLinkElements().Is(
                "rel:canonical, href:/counter, type:, media:, title:link-B, sizes:",
                "rel:icon, href:/_content/SampleSite.Components/favicons/counter-0.png, type:image/png, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );

            // Go back to "Home", and validate link elements of "Home" were restored.
            driver.ClickHome();
            driver.DumpLinkElements().Is(
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );
        }

        [Theory(DisplayName = "Refresh on Browser (from Home)")]
        [MemberData(nameof(HostingModels))]
        public void Refresh_on_Browser_Start_from_Home_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel));
            driver.ClickRedirect();
            driver.DumpMetaElements()
                .Contains("'','','refresh','3;url=/'") // <- added 'refresh'
                .IsTrue();

            // Wait for redirected...
            Thread.Sleep(5000);

            // Validate current page is "Home".
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Hello, world!']")));
            driver.Url.TrimEnd('/').Is(_TestContext.GetHostUrl(hostingModel).TrimEnd('/'));
        }

        [Theory(DisplayName = "Refresh and Cancel on Browser (from Home)")]
        [MemberData(nameof(HostingModels))]
        public void Refresh_and_Cancel_on_Browser_Start_from_Home_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel));
            driver.ClickRedirect();
            driver.DumpMetaElements()
                .Contains("'','','refresh','3;url=/'") // <- added 'refresh'
                .IsTrue();

            // Navigate to "Counter"
            driver.ClickCounter();

            // Wait for redirected...
            Thread.Sleep(5000);

            // Validate current page is not redirected, stay on "Counter".
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Counter']")));
            driver.Url.TrimEnd('/').Is(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/counter");
        }

        [Theory(DisplayName = "Refresh on Browser (from Redirect)")]
        [MemberData(nameof(HostingModels))]
        public void Refresh_on_Browser_Start_from_Redirect_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            driver.Navigate().GoToUrl(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/redirect");
            driver.Wait(5000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Redirect to Home']")));
            Thread.Sleep(200);
            driver.DumpMetaElements()
                .Contains("'','','refresh','3;url=/'") // <- added 'refresh'
                .IsTrue();

            // Wait for redirected...
            Thread.Sleep(5000);

            // Validate current page is "Home".
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Hello, world!']")));
            driver.Url.TrimEnd('/').Is(_TestContext.GetHostUrl(hostingModel).TrimEnd('/'));
        }

        [Theory(DisplayName = "Refresh and Cancel on Browser (from Redirect)")]
        [MemberData(nameof(HostingModels))]
        public void Refresh_and_Cancel_on_Browser_Start_from_Redirect_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            driver.Navigate().GoToUrl(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/redirect");
            driver.Wait(5000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Redirect to Home']")));
            Thread.Sleep(200);
            driver.DumpMetaElements()
                .Contains("'','','refresh','3;url=/'") // <- added 'refresh'
                .IsTrue();

            // Navigate to "Fetch data"
            driver.ClickFetchData();

            // Wait for redirected...
            Thread.Sleep(5000);

            // Validate current page is not redirected, stay on "Counter".
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Weather forecast']")));
            driver.Url.TrimEnd('/').Is(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/fetchdata");
        }
    }
}
