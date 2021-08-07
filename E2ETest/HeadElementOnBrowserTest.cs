using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HeadElement.E2ETest.Internals;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Xunit;
using static HeadElement.E2ETest.Internals.BlazorVersion;
using static HeadElement.E2ETest.Internals.HostingModel;

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

        private static readonly HostingModel[] _HostingModels = new[] {
            Wasm,
            WasmHosted,
            Server 
        };

        private static readonly BlazorVersion[] _BlazorVersions = new[] {
            NETCore31,
            NET50,
            // NET60 
        };

        public static IEnumerable<object[]> TestCases => _HostingModels.SelectMany(m => _BlazorVersions.Select(v => new object[] { m, v }));

        [Theory(DisplayName = "Change Title on Browser (from Home)")]
        [MemberData(nameof(TestCases))]
        public void ChangeTitle_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to Home
            driver.GoToUrlAndWait(host.GetUrl("/"));

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
        [MemberData(nameof(TestCases))]
        public void ChangeTitle_on_Browser_Start_from_Counter_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to "Counter"
            driver.GoToUrlAndWait(host.GetUrl("/counter"));

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
        [MemberData(nameof(TestCases))]
        public void ChangeMetaElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to Home
            driver.GoToUrlAndWait(host.GetUrl("/"));

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
        [MemberData(nameof(TestCases))]
        public void ChangeMetaElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to "Counter"
            driver.GoToUrlAndWait(host.GetUrl("/counter"));

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
        [MemberData(nameof(TestCases))]
        public void ChangeLinkElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to "Home", and validate link elements of "Home"
            driver.GoToUrlAndWait(host.GetUrl("/"));
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
        [MemberData(nameof(TestCases))]
        public void ChangeLinkElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to "Counter", and validate link elements of "Counter"
            driver.GoToUrlAndWait(host.GetUrl("/counter"));
            var actualAtCounter = driver.DumpLinkElements();
            actualAtCounter.Is(ExpectLinks.AtCounter);

            // Go back to "Home", and validate link elements of "Home" were restored.
            driver.ClickHome();
            var actualAtHomem = driver.DumpLinkElements();
            actualAtHomem.Is(ExpectLinks.AtHome);
        }

        [Theory(DisplayName = "Refresh on Browser (from Home)")]
        [MemberData(nameof(TestCases))]
        public void Refresh_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            driver.GoToUrlAndWait(host.GetUrl("/"));
            driver.ClickRedirect();
            driver.DumpMetaElements()
                .Contains("'','','refresh','3;url=/'") // <- added 'refresh'
                .IsTrue();

            // Wait for redirected...
            Thread.Sleep(5000);

            // Validate current page is "Home".
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Hello, world!']")));
            driver.Url.TrimEnd('/').Is(host.GetUrl("/").TrimEnd('/'));
        }

        [Theory(DisplayName = "Refresh and Cancel on Browser (from Home)")]
        [MemberData(nameof(TestCases))]
        public void Refresh_and_Cancel_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            driver.GoToUrlAndWait(host.GetUrl("/"));
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
            driver.Url.TrimEnd('/').Is(host.GetUrl("/").TrimEnd('/') + "/counter");
        }

        [Theory(DisplayName = "Refresh on Browser (from Redirect)")]
        [MemberData(nameof(TestCases))]
        public void Refresh_on_Browser_Start_from_Redirect_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            driver.Navigate().GoToUrl(host.GetUrl("/").TrimEnd('/') + "/redirect");
            driver.Wait(5000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Redirect to Home']")));
            Thread.Sleep(200);
            driver.DumpMetaElements()
                .Contains("'','','refresh','3;url=/'") // <- added 'refresh'
                .IsTrue();

            // Wait for redirected...
            Thread.Sleep(5000);

            // Validate current page is "Home".
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Hello, world!']")));
            driver.Url.TrimEnd('/').Is(host.GetUrl("/").TrimEnd('/'));
        }

        [Theory(DisplayName = "Refresh and Cancel on Browser (from Redirect)")]
        [MemberData(nameof(TestCases))]
        public void Refresh_and_Cancel_on_Browser_Start_from_Redirect_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            driver.Navigate().GoToUrl(host.GetUrl("/").TrimEnd('/') + "/redirect");
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
            driver.Url.TrimEnd('/').Is(host.GetUrl("/").TrimEnd('/') + "/fetchdata");
        }

        [Theory(DisplayName = "Change at OnAfterRender on Browser (from Home)")]
        [MemberData(nameof(TestCases))]
        public void Change_at_OnAfterRender_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to Home, and validate the document title of "Home"
            driver.GoToUrlAndWait(host.GetUrl("/"));
            driver.Wait(1000).Until(_ => driver.Title == "Sample Site");

            // Navigate to "Change at "OnAfterRender"", and validate the document title of it
            driver.ClickOnAfterRender();
            driver.Wait(1000).Until(_ => driver.Title == "2nd title");

            // Validate meta elements of "Change at "OnAfterRender""
            var actualMetaAtOnAfterRender = driver.DumpMetaElements();
            actualMetaAtOnAfterRender.Is(ExpectMeta.AtOnAfterRender);

            // Validate link elements of "Change at "OnAfterRender""
            var actualLinkAtOnAfterRender = driver.DumpLinkElements();
            actualLinkAtOnAfterRender.Is(ExpectLinks.AtOnAfterRender);

            // Go back to Home, and validate the document title, meta elements, link elements of "Home"
            driver.ClickHome();
            driver.Wait(1000).Until(_ => driver.Title == "Sample Site");
            var actualMetaAtHome = driver.DumpMetaElements();
            actualMetaAtHome.Is(ExpectMeta.AtHome);
            var actualLinkAtHome = driver.DumpLinkElements();
            actualLinkAtHome.Is(ExpectLinks.AtHome);
        }

        [Theory(DisplayName = "Change at OnAfterRender on Browser (from OnAfterRender)")]
        [MemberData(nameof(TestCases))]
        public void Change_at_OnAfterRender_on_Browser_Start_from_OnAfterRender_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;

            // Navigate to "Change at "OnAfterRender"", and validate the document title of it
            driver.GoToUrlAndWait(host.GetUrl("/change-at-onafterrender"));
            driver.Wait(5000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Change at \"OnAfterRender\"']")));
            Thread.Sleep(200);

            driver.Wait(1000).Until(_ => driver.Title == "2nd title");

            // Validate meta elements of "Change at "OnAfterRender""
            var actualMetaAtOnAfterRender = driver.DumpMetaElements();
            actualMetaAtOnAfterRender.Is(ExpectMeta.AtOnAfterRender);

            // Validate link elements of "Change at "OnAfterRender""
            var actualLinkAtOnAfterRender = driver.DumpLinkElements();
            actualLinkAtOnAfterRender.Is(ExpectLinks.AtOnAfterRender);

            // Go back to Home, and validate the document title, meta elements, link elements of "Home"
            driver.ClickHome();
            driver.Wait(1000).Until(_ => driver.Title == "Sample Site");
            var actualMetaAtHome = driver.DumpMetaElements();
            actualMetaAtHome.Is(ExpectMeta.AtHome);
            var actualLinkAtHome = driver.DumpLinkElements();
            actualLinkAtHome.Is(ExpectLinks.AtHome);
        }

        [Theory(DisplayName = "Check another helper script in the same namespace should not be overridden")]
        [MemberData(nameof(TestCases))]
        public void HelperJavaScript_Namespace_Not_Conflict_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
        {
            var host = this._TestContext.StartHost(hostingModel, blazorVersion);
            var driver = this._TestContext.WebDriver;
            driver.GoToUrlAndWait(host.GetUrl("/counter"));

            var random = new Random();
            var a = random.Next(1, 10);
            var b = random.Next(1, 10);
            var result = driver.ExecuteJavaScript<long>($"return Toolbelt.Blazor.add({a},{b})");

            result.Is(a + b);
        }
    }
}
