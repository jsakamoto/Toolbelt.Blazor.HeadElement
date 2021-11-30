using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace HeadElement.E2ETest
{
    public static class WebDriverExtensions
    {
        public static void GoToUrlAndWait(this IWebDriver driver, string url, string path = "")
        {
            driver.Navigate().GoToUrl(url.TrimEnd('/') + "/" + path.TrimStart('/'));
            driver.Wait(5000).Until(_ => driver.FindElement(By.CssSelector("a.navbar-brand")));
            driver.Wait(5000).Until(_ => driver.FindElements(By.CssSelector(".loading")).Count == 0);
            Thread.Sleep(200);
        }

        public static WebDriverWait Wait(this IWebDriver driver, int millisecondsTimeout)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(millisecondsTimeout));
            wait.PollingInterval = TimeSpan.FromMilliseconds(200);
            return wait;
        }

        public static void ClickCounter(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("a[href=counter]")).Click();
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Counter']")));
            Thread.Sleep(200);
        }

        public static void ClickFetchData(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("a[href=fetchdata]")).Click();
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Weather forecast']")));
            Thread.Sleep(200);
        }

        public static void ClickOnAfterRender(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("a[href=change-at-onafterrender]")).Click();
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Change at \"OnAfterRender\"']")));
            Thread.Sleep(200);
        }

        public static void ClickHome(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("a.navbar-brand")).Click();
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Hello, world!']")));
            Thread.Sleep(200);
        }

        public static void ClickRedirect(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("a[href=redirect]")).Click();
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Redirect to Home']")));
            Thread.Sleep(200);
        }

        public static string[] DumpMetaElements(this IWebDriver driver)
        {
            var script = "return JSON.stringify(" +
                "Array.from(document.querySelectorAll('meta'))" +
                ".map(m=>[m.name, m.getAttribute('property'), m.httpEquiv, m.content])" +
            ")";
            var metaJson = driver.ExecuteJavaScript<string>(script);
            var metaArray = System.Text.Json.JsonSerializer.Deserialize<string[][]>(metaJson);
            var metaElements = metaArray
                .Select(m => (
                    Name: m[0] ?? "",
                    Property: m[1] ?? "",
                    HttpEquiv: m[2] ?? "",
                    Content: m[3] ?? ""))
                .OrderBy(m => m.HttpEquiv).ThenBy(m => m.Property).ThenBy(m => m.Name)
                .Select(m => $"'{m.Name}','{m.Property}','{m.HttpEquiv}','{m.Content}'")
                .ToArray();
            return metaElements;
        }

        public static string[] DumpLinkElements(this IWebDriver driver)
        {
            var script = "return JSON.stringify(" +
                "Array.from(document.querySelectorAll('link'))" +
                ".map(l=>[l.rel, l.href, l.type, l.media, l.title, ''+l.sizes, l.as, l.crossOrigin||'', l.hreflang, l.imageSizes, l.imageSrcset, ''+l.disabled])" +
            ")";
            var linksJson = driver.ExecuteJavaScript<string>(script);
            var linksArray = System.Text.Json.JsonSerializer.Deserialize<string[][]>(linksJson);
            var linkElements = linksArray
                .Select(l => (
                    Rel: l[0] ?? "",
                    Href: Uri.TryCreate(l[1], UriKind.Absolute, out var u) ? u.PathAndQuery : "",
                    Type: l[2] ?? "",
                    Media: l[3] ?? "",
                    Title: l[4] ?? "",
                    Sizes: l[5] ?? "",
                    As: l[6] ?? "",
                    CrossOrigin: l[7] ?? "",
                    Hreflang: l[8] ?? "",
                    ImageSizes: l[9] ?? "",
                    ImageSrcset: l[10] ?? "",
                    Disabled: l[11] ?? ""))
                .OrderBy(l => l.Rel).ThenBy(l => l.Href).ThenBy(l => l.Media).ThenBy(l => l.Hreflang)
                .Select(l => $"rel:{l.Rel}, href:{l.Href}, type:{l.Type}, media:{l.Media}, title:{l.Title}, sizes:{l.Sizes}, as:{l.As}, crossorigin:{l.CrossOrigin}, hreflang:{l.Hreflang}, imagesizes:{l.ImageSizes}, imagesrcset:{l.ImageSrcset}, disabled:{l.Disabled}")
                .ToArray();
            return linkElements;
        }
    }
}
