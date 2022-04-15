using System.Text.RegularExpressions;
using HeadElement.E2ETest.Internals;
using Xunit;
using static HeadElement.E2ETest.Internals.BlazorVersion;
using static HeadElement.E2ETest.Internals.HostingModel;

namespace HeadElement.E2ETest;

[Collection(nameof(TestContext))]
public class HeadElementServerPreRenderingTest
{
    private readonly TestContext _TestContext;

    public HeadElementServerPreRenderingTest(TestContext testContext)
    {
        this._TestContext = testContext;
    }

    private static readonly HostingModel[] _HostingModels = new[] {
        WasmHosted,
        Server
    };

    private static readonly BlazorVersion[] _BlazorVersions = new[] {
        NETCore31,
        NET50,
        NET60
    };

    public static IEnumerable<object[]> TestCases => _HostingModels.SelectMany(m => _BlazorVersions.Select(v => new object[] { m, v }));

    [Theory(DisplayName = "Change Title on Server")]
    [MemberData(nameof(TestCases))]
    public async Task ChangeTitle_on_Server_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
    {
        var host = await this._TestContext.StartHostAsync(hostingModel, blazorVersion);

        var httpClient = new HttpClient();

        var contentOfHome = await httpClient.GetStringAsync(host.GetUrl("/"));
        Regex.Match(contentOfHome, "<title>(?<title>.+)</title>")
            .Groups["title"].Value.Is("Sample Site");

        var contentOfCounter = await httpClient.GetStringAsync(host.GetUrl("/").TrimEnd('/') + "/counter");
        Regex.Match(contentOfCounter, "<title>(?<title>.+)</title>")
            .Groups["title"].Value.Is("Counter(0)");

        var contentOfFetchdata = await httpClient.GetStringAsync(host.GetUrl("/").TrimEnd('/') + "/fetchdata");
        Regex.Match(contentOfFetchdata, "<title>(?<title>.+)</title>")
            .Groups["title"].Value.Is("Fetch data");
    }

    [Theory(DisplayName = "Change meta elements on Server")]
    [MemberData(nameof(TestCases))]
    public async Task ChangeMetaElements_on_Server_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
    {
        var host = await this._TestContext.StartHostAsync(hostingModel, blazorVersion);
        var httpClient = new HttpClient();

        var contentAtHome = await httpClient.GetStringAsync(host.GetUrl("/"));
        var actualAtHome = this.DumpMetaElements(contentAtHome);
        actualAtHome.Is(ExpectMeta.AtHome);

        var contentAtCounter = await httpClient.GetStringAsync(host.GetUrl("/counter"));
        var actualAtCounter = this.DumpMetaElements(contentAtCounter);
        actualAtCounter.Is(ExpectMeta.AtCounter);

        var contentAtFetchdata = await httpClient.GetStringAsync(host.GetUrl("/fetchdata"));
        var actualAtFetchData = this.DumpMetaElements(contentAtFetchdata);
        actualAtFetchData.Is(ExpectMeta.AtFetchData);
    }

    private IEnumerable<string> DumpMetaElements(string content)
    {
        // remove comments.
        content = Regex.Replace(content, "<!--.+?-->", "");
        return Regex.Matches(content, @"<meta[ \t]+[^>]*>")
            .Select(m => (
                Name: Regex.Match(m.Value, "name=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Property: Regex.Match(m.Value, "property=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                HttpEquiv: Regex.Match(m.Value, "http\\-equiv=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Media: Regex.Match(m.Value, "media=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Content: Regex.Match(m.Value, "content=\"(?<t>[^\"]+)\"").Groups["t"].Value
            ))
            .OrderBy(m => m.HttpEquiv).ThenBy(m => m.Property).ThenBy(m => m.Name).ThenBy(m => m.Media)
            .Select(m => $"{m.Name}|{m.Property}|{m.HttpEquiv}|{m.Media}|{m.Content}")
            .ToArray();
    }

    [Theory(DisplayName = "Change link elements on Server")]
    [MemberData(nameof(TestCases))]
    public async Task ChangeLinkElements_on_Server_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
    {
        var host = await this._TestContext.StartHostAsync(hostingModel, blazorVersion);
        var httpClient = new HttpClient();

        var contentAtHome = await httpClient.GetStringAsync(host.GetUrl("/"));
        var actualAtHome = this.DumpLinkElements(contentAtHome);
        actualAtHome.Is(ExpectLinks.AtHome);

        var contentAtCounter = await httpClient.GetStringAsync(host.GetUrl("/counter"));
        var actualAtCounter = this.DumpLinkElements(contentAtCounter);
        actualAtCounter.Is(ExpectLinks.AtCounter);

        var contentAtFetchdata = await httpClient.GetStringAsync(host.GetUrl("/fetchdata"));
        var actualAtFetchData = this.DumpLinkElements(contentAtFetchdata);
        actualAtFetchData.Is(ExpectLinks.AtFetchData);
    }

    [Theory(DisplayName = "Add link elements only on Server")]
    [MemberData(nameof(TestCases))]
    public async Task AddLinkElementsOnly_on_Server_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
    {
        var host = await this._TestContext.StartHostAsync(hostingModel, blazorVersion);
        var httpClient = new HttpClient();
        var contentOfCanonical = await httpClient.GetStringAsync(host.GetUrl("/canonical"));

        this.DumpLinkElements(contentOfCanonical).Any(
            elemnt => elemnt == $"rel:canonical, href:/canonical, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false"
        ).IsTrue();
    }

    private IEnumerable<string> DumpLinkElements(string content)
    {
        static string makeHref(string href) => Uri.TryCreate(href, UriKind.Absolute, out var u) ? u.PathAndQuery : "/" + href.TrimStart('/');

        // remove comments.
        content = Regex.Replace(content, "<!--.+?-->", "");

        return Regex.Matches(content, @"<link[ \t]+[^>]*>")
            .Select(l => (
                Rel: Regex.Match(l.Value, "rel=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Href: makeHref(Regex.Match(l.Value, "href=\"(?<t>[^\"]+)\"").Groups["t"].Value),
                Type: Regex.Match(l.Value, "[ ]+type=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Media: Regex.Match(l.Value, "[ ]+media=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Title: Regex.Match(l.Value, "[ ]+title=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Sizes: Regex.Match(l.Value, "[ ]+sizes=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                As: Regex.Match(l.Value, "[ ]+as=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                CrossOrigin: Regex.Match(l.Value, "[ ]+crossorigin=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Hreflang: Regex.Match(l.Value, "[ ]+hreflang=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                ImageSizes: Regex.Match(l.Value, "[ ]+imagesizes=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                ImageSrcset: Regex.Match(l.Value, "[ ]+imagesrcset=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                Disabled: Regex.Match(l.Value, @"[ ]+disabled[ /]+").Success ? "true" : "false"
            ))
            .Where(l => !l.Href.Contains("text/css,.loading%7B")) // exclude style for "Loading..." element.
            .OrderBy(l => l.Rel).ThenBy(l => l.Href).ThenBy(l => l.Media)
            .Select(l => $"rel:{l.Rel}, href:{l.Href}, type:{l.Type}, media:{l.Media}, title:{l.Title}, sizes:{l.Sizes}, as:{l.As}, crossorigin:{l.CrossOrigin}, hreflang:{l.Hreflang}, imagesizes:{l.ImageSizes}, imagesrcset:{l.ImageSrcset}, disabled:{l.Disabled}")
            .ToArray();
    }

    [Theory(DisplayName = "Change at OnAfterRender on Server")]
    [MemberData(nameof(TestCases))]
    public async Task Change_at_OnAfterRender_on_Server_Test(HostingModel hostingModel, BlazorVersion blazorVersion)
    {
        var host = await this._TestContext.StartHostAsync(hostingModel, blazorVersion);
        var httpClient = new HttpClient();
        var contentAtOnAfterRender = await httpClient.GetStringAsync(host.GetUrl("/change-at-onafterrender"));

        Regex.Match(contentAtOnAfterRender, "<title>(?<title>.+)</title>")
            .Groups["title"].Value.Is("1st title");

        // Validate meta elements of "Change at "OnAfterRender""
        var actualMetaAtOnAfterRender = this.DumpMetaElements(contentAtOnAfterRender);
        actualMetaAtOnAfterRender.Is(ExpectMeta.AtOnAfterRenderPrerendered);

        // Validate link elements of "Change at "OnAfterRender""
        var actualLinkAtOnAfterRender = this.DumpLinkElements(contentAtOnAfterRender);
        actualLinkAtOnAfterRender.Is(ExpectLinks.AtOnAfterRenderPrerendered);
    }
}
