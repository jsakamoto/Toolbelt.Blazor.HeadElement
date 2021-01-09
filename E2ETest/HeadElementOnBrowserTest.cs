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
            var actualAtHome = driver.DumpMetaElements();
            actualAtHome.Is(ExpectMeta.AtHome);

            // Navigate to "Counter"
            driver.ClickCounter();

            // Validate meta elements of "Counter"
            var actualAtCounter = driver.DumpMetaElements();
            actualAtCounter.Is(ExpectMeta.AtCounter);

            // Navigate to "Fetch data"
            driver.ClickFetchData();

            // Validate meta elements of "Fetch data"
            var actualAtFetchData = driver.DumpMetaElements();
            actualAtFetchData.Is(ExpectMeta.AtFetchData);

            // Go back to "Home"
            driver.ClickHome();

            // Validate meta elements of "Home" were restored.
            var actualAtReturnHome = driver.DumpMetaElements();
            actualAtReturnHome.Is(ExpectMeta.AtHome);
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
            var actualAtCounter = driver.DumpMetaElements();
            actualAtCounter.Is(ExpectMeta.AtCounter);

            // Go back to "Home"
            driver.ClickHome();

            // Validate meta elements of "Home" were restored.
            var actualAtHome = driver.DumpMetaElements();
            actualAtHome.Is(ExpectMeta.AtHome);
        }


        [Theory(DisplayName = "Change link elements on Browser (from Home)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeLinkElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to "Home", and validate link elements of "Home"
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel));
            var actualAtHome = driver.DumpLinkElements();
            actualAtHome.Is(ExpectLinks.AtHome);

            // Navigate to "Counter", and validate link elements of "Counter"
            driver.ClickCounter();
            var actualAtCounter = driver.DumpLinkElements();
            actualAtCounter.Is(ExpectLinks.AtCounter);

            // Navigate to "Fetch data", and validate link elements of "Fetch data"
            driver.ClickFetchData();
            var actualAtFetchData = driver.DumpLinkElements();
            actualAtFetchData.Is(ExpectLinks.AtFetchData);

            // Go back to "Home", and validate link elements of "Home" were restored.
            driver.ClickHome();
            var actualAtReturnHome = driver.DumpLinkElements();
            actualAtReturnHome.Is(ExpectLinks.AtHome);
        }

        [Theory(DisplayName = "Change link elements on Browser (from Counter)")]
        [MemberData(nameof(HostingModels))]
        public void ChangeLinkElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var driver = _TestContext.WebDriver;

            // Navigate to "Counter", and validate link elements of "Counter"
            driver.GoToUrlAndWait(_TestContext.GetHostUrl(hostingModel), "/counter");
            var actualAtCounter = driver.DumpLinkElements();
            actualAtCounter.Is(ExpectLinks.AtCounter);

            // Go back to "Home", and validate link elements of "Home" were restored.
            driver.ClickHome();
            var actualAtHomem = driver.DumpLinkElements();
            actualAtHomem.Is(ExpectLinks.AtHome);
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
