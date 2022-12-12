using System.Text.Json;
using Microsoft.Playwright;
using NUnit.Framework;

namespace HeadElement.E2ETest.Internals;

public static class PlaywrightExtensions
{
    public static async ValueTask GotoAndWaitForReadyAsync(this IPage page, string url)
    {
        var waiter = page.WaitForBlazorHasBeenStarted();
        await page.GotoAsync(url);
        await waiter;
    }

    public static async ValueTask WaitForAsync(this IPage page, Func<IPage, ValueTask<bool>> predictAsync, bool throwOnTimeout = true)
    {
        var canceller = new CancellationTokenSource(millisecondsDelay: 5000);
        do
        {
            if (await predictAsync(page)) return;
            await Task.Delay(100);
        } while (!canceller.IsCancellationRequested);
        if (throwOnTimeout) throw new OperationCanceledException(canceller.Token);
    }

    public static async ValueTask WaitForBlazorHasBeenStarted(this IPage page)
    {
        await page.WaitForConsoleMessageAsync(new() { Predicate = message => message.Text == "Blazor has been started." });
    }

    public static async ValueTask AssertTitleIsAsync(this IPage page, string expectedTitle)
    {
        await page.AssertEqualsAsync(_ => _.TitleAsync(), expectedTitle);
    }

    public static async ValueTask AssertUrlIsAsync(this IPage page, string expectedUrl)
    {
        expectedUrl = expectedUrl.TrimEnd('/');
        await page.AssertEqualsAsync(async _ =>
        {
            var href = await _.EvaluateAsync<string>("window.location.href");
            return href.TrimEnd('/');
        }, expectedUrl);
    }

    public static async ValueTask AssertH1IsAsync(this IPage page, string expectedH1Text)
    {
        var h1 = page.Locator("h1");
        await page.AssertEqualsAsync(_ => h1.TextContentAsync(), expectedH1Text);
    }

    public static async ValueTask AssertItsTrueAsync<T>(this IPage page, Func<IPage, Task<T>> selector, Func<T, bool> predicate)
    {
        try
        {
            await page.WaitForAsync(async p =>
            {
                var value = await selector(p);
                return predicate.Invoke(value);
            });
        }
        catch (OperationCanceledException) { (false).IsTrue(); }
    }

    public static async ValueTask AssertEqualsAsync<T>(this IPage page, Func<IPage, Task<IEnumerable<T>>> selector, IEnumerable<T> expectedValue)
    {
        var actualValue = Enumerable.Empty<T>();
        await page.WaitForAsync(async p =>
        {
            actualValue = await selector.Invoke(p);
            return Enumerable.SequenceEqual(actualValue, expectedValue);
        }, throwOnTimeout: false);
        actualValue.Is(expectedValue);
    }

    public static async ValueTask AssertEqualsAsync<T>(this IPage page, Func<IPage, Task<T>> selector, T expectedValue)
    {
        var actualValue = default(T);
        await page.WaitForAsync(async p =>
        {
            actualValue = await selector.Invoke(p);
            return actualValue!.Equals(expectedValue);
        }, throwOnTimeout: false);
        actualValue.Is(expectedValue);
    }

    public static async ValueTask ClickNavigationAsync(this IPage page, string href, string h1text)
    {
        var selector = href == "" || href == "/" ? "a.navbar-brand" : $"a[href={href}]";
        await page.ClickAsync(selector);
        await page.AssertH1IsAsync(h1text);
    }

    public static ValueTask ClickHomeAsync(this IPage page) => page.ClickNavigationAsync("/", "Hello, world!");

    public static ValueTask ClickCounterAsync(this IPage page) => page.ClickNavigationAsync("counter", "Counter");

    public static ValueTask ClickFetchDataAsync(this IPage page) => page.ClickNavigationAsync("fetchdata", "Weather forecast");

    public static ValueTask ClickRedirectAsync(this IPage page) => page.ClickNavigationAsync("redirect", "Redirect to Home");

    public static ValueTask ClickOnAfterRenderAsync(this IPage page) => page.ClickNavigationAsync("change-at-onafterrender", "Change at \"OnAfterRender\"");

    private record MetaElement(string Name, string Property, string HttpEquiv, string Media, string Content);

    public static async Task<IEnumerable<string>> DumpMetaElementsAsync(this IPage page)
    {
        var script = "JSON.stringify(" +
            "Array.from(document.querySelectorAll('meta'))" +
            ".map(m => ({name: m.name, property: m.getAttribute('property')||'', httpEquiv: m.httpEquiv, media: m.getAttribute('media')||'', content: m.content}))" +
        ")";
        var metaJson = await page.EvaluateAsync<string>(script);
        var metaElements = JsonSerializer.Deserialize<MetaElement[]>(metaJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? Enumerable.Empty<MetaElement>();
        var dump = metaElements
            .OrderBy(m => m.HttpEquiv).ThenBy(m => m.Property).ThenBy(m => m.Name).ThenBy(m => m.Media)
            .Select(m => $"{m.Name}|{m.Property}|{m.HttpEquiv}|{m.Media}|{m.Content}")
            .ToArray();
        return dump;
    }

    private record LinkElement(string Rel, string Href, string Type, string Media, string Title, string Sizes, string As, string CrossOrigin, string Hreflang, string ImageSizes, string ImageSrcset, string Disabled);

    public static async Task<IEnumerable<string>> DumpLinkElementsAsync(this IPage page)
    {
        static string toPathAndQuery(string href) => Uri.TryCreate(href, UriKind.Absolute, out var u) ? u.PathAndQuery : "";

        var script = "JSON.stringify(" +
            "Array.from(document.querySelectorAll('link'))" +
            ".map(l=>({rel: l.rel, href: l.href, type: l.type, media: l.media, title: l.title, sizes: ''+l.sizes, as: l.as, crossOrigin: l.crossOrigin||'', hreflang: l.hreflang, imageSizes: l.imageSizes, imageSrcset: l.imageSrcset, disabled: ''+l.disabled}))" +
        ")";

        var linksJson = await page.EvaluateAsync<string>(script);
        var linksElements = JsonSerializer.Deserialize<LinkElement[]>(linksJson, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? Array.Empty<LinkElement>();
        var dump = linksElements

            // NOTE: Exclude <link rel="modulepreload" href="_framework/dotnet...js" .../>
            //       that is automatically injected by Blazor Wasm Runtime,
            //       because it is a noise for this E2E test.
            .Where(l => !(l.Rel == "modulepreload" && toPathAndQuery(l.Href).StartsWith("/_framework/dotnet.")))

            .OrderBy(l => l.Rel).ThenBy(l => l.Href).ThenBy(l => l.Media).ThenBy(l => l.Hreflang)
            .Select(l => $"rel:{l.Rel}, href:{toPathAndQuery(l.Href)}, type:{l.Type}, media:{l.Media}, title:{l.Title}, sizes:{l.Sizes}, as:{l.As}, crossorigin:{l.CrossOrigin}, hreflang:{l.Hreflang}, imagesizes:{l.ImageSizes}, imagesrcset:{l.ImageSrcset}, disabled:{l.Disabled}")
            .ToArray();
        return dump;
    }
}
