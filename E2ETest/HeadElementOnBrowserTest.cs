using HeadElement.E2ETest.Internals;
using NUnit.Framework;

namespace HeadElement.E2ETest;

public class HeadElementOnBrowserTest
{
    private TestContext TestContext => TestContext.Default!;

    public static IEnumerable<object[]> TestCases => TestContext.SampleSites.Keys
        .Select(key => new object[] { key.HostingModel, key.BlazorVersion, key.DisableScriptInjection });

    [@TestCaseSource(nameof(TestCases), TestName = "Change Title on Browser (from Home)")]
    public async Task ChangeTitle_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to Home
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/"));

        // Validate document title of "Home"
        await page.AssertTitleIsAsync("Sample Site");

        // Navigate to "Counter"
        await page.ClickCounterAsync();

        // Validate document title of "Counter"
        await page.AssertTitleIsAsync("Counter(0)");

        // document title will be updated real time.
        await page.ClickAsync("button.btn-primary");
        await page.AssertTitleIsAsync("Counter(1)");
        await page.ClickAsync("button.btn-primary");
        await page.AssertTitleIsAsync("Counter(2)");

        // Navigate to "Fetch data"
        await page.ClickFetchDataAsync();

        // Validate document title of "Fetch data"
        await page.AssertTitleIsAsync("Fetch data");

        // Go back to "Home"
        await page.ClickHomeAsync();

        // Validate document title of "Home" was restored.
        await page.AssertTitleIsAsync("Sample Site");
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Change Title on Browser (from Counter)")]
    public async Task ChangeTitle_on_Browser_Start_from_Counter_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to "Counter"
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/counter"));

        // Validate document title of "Counter"
        await page.AssertTitleIsAsync("Counter(0)");

        // document title will be updated real time.
        await page.ClickAsync("button.btn-primary");
        await page.AssertTitleIsAsync("Counter(1)");
        await page.ClickAsync("button.btn-primary");
        await page.AssertTitleIsAsync("Counter(2)");

        // Go back to "Home"
        await page.ClickHomeAsync();

        // Validate document title of "Home" was restored.
        await page.AssertTitleIsAsync("Sample Site");
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Change meta elements on Browser (from Home)")]
    public async Task ChangeMetaElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to Home
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/"));

        // Validate meta elements of "Home"
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtHome);

        // Navigate to "Counter"
        await page.ClickCounterAsync();

        // Validate meta elements of "Counter"
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtCounter);

        // Navigate to "Fetch data"
        await page.ClickFetchDataAsync();

        // Validate meta elements of "Fetch data"
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtFetchData);

        // Go back to "Home"
        await page.ClickHomeAsync();

        // Validate meta elements of "Home" were restored.
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtHome);
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Change meta elements on Browser (from Counter)")]
    public async Task ChangeMetaElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to "Counter"
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/counter"));

        // Validate meta elements of "Counter"
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtCounter);

        // Go back to "Home"
        await page.ClickHomeAsync();

        // Validate meta elements of "Home" were restored.
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtHome);
    }


    [@TestCaseSource(nameof(TestCases), TestName = "Change link elements on Browser (from Home)")]
    public async Task ChangeLinkElements_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to "Home", and validate link elements of "Home"
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/"));
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtHome);

        // Navigate to "Counter", and validate link elements of "Counter"
        await page.ClickCounterAsync();
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtCounter);

        // Navigate to "Fetch data", and validate link elements of "Fetch data"
        await page.ClickFetchDataAsync();
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtFetchData);

        // Go back to "Home", and validate link elements of "Home" were restored.
        await page.ClickHomeAsync();
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtHome);
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Change link elements on Browser (from Counter)")]
    public async Task ChangeLinkElements_on_Browser_Start_from_Counter_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to "Counter", and validate link elements of "Counter"
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/counter"));
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtCounter);

        // Go back to "Home", and validate link elements of "Home" were restored.
        await page.ClickHomeAsync();
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtHome);
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Refresh on Browser (from Home)")]
    public async Task Refresh_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        await page.GotoAndWaitForReadyAsync(host.GetUrl("/"));
        await page.ClickRedirectAsync();
        var metaElements = await page.DumpMetaElementsAsync();
        await page.AssertItsTrueAsync(_ => _.DumpMetaElementsAsync(), m => m.Contains("||refresh||3;url=/")); //<- added 'refresh'

        // Wait for redirected...
        await Task.Delay(4000);

        // Validate current page is "Home".
        await page.AssertH1IsAsync("Hello, world!");
        var url = await page.EvaluateAsync<string>("window.location.href");
        url.TrimEnd('/').Is(host.GetUrl("/").TrimEnd('/'));
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Refresh and Cancel on Browser (from Home)")]
    public async Task Refresh_and_Cancel_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        await page.GotoAndWaitForReadyAsync(host.GetUrl("/"));
        await page.ClickRedirectAsync();
        await page.AssertItsTrueAsync(_ => _.DumpMetaElementsAsync(), m => m.Contains("||refresh||3;url=/")); //<- added 'refresh'

        // Navigate to "Counter"
        await page.ClickCounterAsync();

        // Wait for redirected...
        await Task.Delay(4000);

        // Validate current page is not redirected, stay on "Counter".
        await page.AssertH1IsAsync("Counter");
        var url = await page.EvaluateAsync<string>("window.location.href");
        url.TrimEnd('/').Is(host.GetUrl("/counter").TrimEnd('/'));
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Refresh on Browser (from Redirect)")]
    public async Task Refresh_on_Browser_Start_from_Redirect_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // NOTICE: in case of pre-rendered Blazor Wasm, the redirection by "meta refresh" happens sometimes 
        // before complete to Blazor Wasm contents loading and initialization.

        await page.GotoAsync(host.GetUrl("/redirect"));
        if (hostingModel == HostingModel.Wasm) await page.WaitForBlazorHasBeenStarted();
        await page.AssertItsTrueAsync(_ => _.DumpMetaElementsAsync(), m => m.Contains("||refresh||3;url=/")); //<- added 'refresh'

        // Wait for redirected...
        if (hostingModel != HostingModel.Wasm) await page.WaitForBlazorHasBeenStarted();
        await Task.Delay(4000);

        // Validate current page is "Home".
        await page.AssertUrlIsAsync(host.GetUrl("/"));
        await page.AssertH1IsAsync("Hello, world!");
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Refresh and Cancel on Browser (from Redirect)")]
    public async Task Refresh_and_Cancel_on_Browser_Start_from_Redirect_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        await page.GotoAndWaitForReadyAsync(host.GetUrl("/redirect"));
        await page.AssertItsTrueAsync(_ => _.DumpMetaElementsAsync(), m => m.Contains("||refresh||3;url=/")); //<- added 'refresh'
        await Task.Delay(1000);

        // Navigate to "Fetch data"
        await page.ClickFetchDataAsync();

        // Wait for redirected...
        await Task.Delay(3000);

        // Validate current page is not redirected, stay on "Weather Forecast".
        await page.AssertH1IsAsync("Weather forecast");
        await page.AssertUrlIsAsync(host.GetUrl("/fetchdata"));
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Change at OnAfterRender on Browser (from Home)")]
    public async Task Change_at_OnAfterRender_on_Browser_Start_from_Home_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to Home, and validate the document title of "Home"
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/"));
        await page.AssertTitleIsAsync("Sample Site");

        // Navigate to "Change at "OnAfterRender"", and validate the document title of it
        await page.ClickOnAfterRenderAsync();
        await page.AssertTitleIsAsync("2nd title");

        // Validate meta elements of "Change at "OnAfterRender""
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtOnAfterRender);

        // Validate link elements of "Change at "OnAfterRender""
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtOnAfterRender);

        // Go back to Home, and validate the document title, meta elements, link elements of "Home"
        await page.ClickHomeAsync();
        await page.AssertTitleIsAsync("Sample Site");
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtHome);
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtHome);
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Change at OnAfterRender on Browser (from OnAfterRender)")]
    public async Task Change_at_OnAfterRender_on_Browser_Start_from_OnAfterRender_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();

        // Navigate to "Change at "OnAfterRender"", and validate the document title of it
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/change-at-onafterrender"));
        await page.AssertH1IsAsync("Change at \"OnAfterRender\"");
        await page.AssertTitleIsAsync("2nd title");

        // Validate meta elements of "Change at "OnAfterRender""
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtOnAfterRender);

        // Validate link elements of "Change at "OnAfterRender""
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtOnAfterRender);

        // Go back to Home, and validate the document title, meta elements, link elements of "Home"
        await page.ClickHomeAsync();
        await page.AssertTitleIsAsync("Sample Site");
        await page.AssertEqualsAsync(_ => _.DumpMetaElementsAsync(), ExpectMeta.AtHome);
        await page.AssertEqualsAsync(_ => _.DumpLinkElementsAsync(), ExpectLinks.AtHome);
    }

    [@TestCaseSource(nameof(TestCases), TestName = "Check another helper script in the same namespace should not be overridden")]
    public async Task HelperJavaScript_Namespace_Not_Conflict_Test(HostingModel hostingModel, BlazorVersion blazorVersion, bool disableScriptInjection)
    {
        var host = await this.TestContext.StartHostAsync(hostingModel, blazorVersion, disableScriptInjection);
        var page = await this.TestContext.GetPageAsync();
        await page.GotoAndWaitForReadyAsync(host.GetUrl("/counter"));

        var random = new Random();
        var a = random.Next(1, 10);
        var b = random.Next(1, 10);
        var result = await page.EvaluateAsync<long>($"Toolbelt.Blazor.add({a},{b})");

        result.Is(a + b);
    }
#if false
#endif
}
