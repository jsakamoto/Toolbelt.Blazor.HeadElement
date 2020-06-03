using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
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
            return new WebDriverWait(driver, TimeSpan.FromMilliseconds(millisecondsTimeout));
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

        public static void ClickHome(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("a.navbar-brand")).Click();
            driver.Wait(1000).Until(_ => driver.FindElement(By.XPath("//h1[text()='Hello, world!']")));
            Thread.Sleep(200);
        }

        public static string[] DumpMetaElements(this IWebDriver driver)
        {
            var metaElements = driver.FindElements(By.XPath("//meta"))
                .Select(m => (
                    Name: m.GetAttribute("name") ?? "",
                    Property: m.GetAttribute("property") ?? "",
                    HttpEquiv: m.GetAttribute("http-equiv") ?? "",
                    Content: m.GetAttribute("content") ?? ""))
                .OrderBy(m => m.HttpEquiv).ThenBy(m => m.Property).ThenBy(m => m.Name)
                .Select(m => $"'{m.Name}','{m.Property}','{m.HttpEquiv}','{m.Content}'")
                .ToArray();
            return metaElements;
        }
    }
}
