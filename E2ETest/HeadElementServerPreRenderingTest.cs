using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace HeadElement.E2ETest
{
    [Collection(nameof(TestContext))]
    public class HeadElementServerPreRenderingTest
    {
        private readonly TestContext _TestContext;

        public HeadElementServerPreRenderingTest(TestContext testContext)
        {
            this._TestContext = testContext;
        }

        public static IEnumerable<object[]> HostingModels { get; } = new List<object[]>
        {
            // new object[] { HostingModel.WasmHosted },
            new object[] { HostingModel.Server },
        };

        [Theory(DisplayName = "Change Title on Server")]
        [MemberData(nameof(HostingModels))]
        public async Task ChangeTitle_on_Server_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);

            var httpClient = new HttpClient();

            var contentOfHome = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel));
            Regex.Match(contentOfHome, "<title>(?<title>.+)</title>")
                .Groups["title"].Value.Is("Sample Site");

            var contentOfCounter = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/counter");
            Regex.Match(contentOfCounter, "<title>(?<title>.+)</title>")
                .Groups["title"].Value.Is("Counter(0)");

            var contentOfFetchdata = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/fetchdata");
            Regex.Match(contentOfFetchdata, "<title>(?<title>.+)</title>")
                .Groups["title"].Value.Is("Fetch data");
        }

        [Theory(DisplayName = "Change meta elements on Server")]
        [MemberData(nameof(HostingModels))]
        public async Task ChangeMetaElements_on_Server_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);

            var httpClient = new HttpClient();

            var contentOfHome = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel));
            DumpMetaElements(contentOfHome).Is(
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

            var contentOfCounter = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/counter");
            DumpMetaElements(contentOfCounter).Is(
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

            var contentOfFetchdata = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/fetchdata");
            DumpMetaElements(contentOfFetchdata).Is(
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
        }

        private IEnumerable<string> DumpMetaElements(string content)
        {
            return Regex.Matches(content, @"<meta[ \t]+[^>]*>")
                .Select(m => (
                    Name: Regex.Match(m.Value, "name=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Property: Regex.Match(m.Value, "property=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    HttpEquiv: Regex.Match(m.Value, "http\\-equiv=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Content: Regex.Match(m.Value, "content=\"(?<t>[^\"]+)\"").Groups["t"].Value
                ))
                .OrderBy(m => m.HttpEquiv).ThenBy(m => m.Property).ThenBy(m => m.Name)
                .Select(m => $"'{m.Name}','{m.Property}','{m.HttpEquiv}','{m.Content}'")
                .ToArray();
        }

        [Theory(DisplayName = "Change link elements on Server")]
        [MemberData(nameof(HostingModels))]
        public async Task ChangeLinkElements_on_Server_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);

            var httpClient = new HttpClient();

            var contentOfHome = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel));
            DumpLinkElements(contentOfHome).Is(
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );

            var contentOfCounter = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/counter");
            DumpLinkElements(contentOfCounter).Is(
                "rel:canonical, href:http://localhost/counter, type:, media:, title:link-B, sizes:",
                "rel:icon, href:/_content/SampleSite.Components/favicons/counter-0.png, type:image/png, media:, title:, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );

            var contentOfFetchdata = await httpClient.GetStringAsync(_TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/fetchdata");
            DumpLinkElements(contentOfFetchdata).Is(
                "rel:canonical, href:http://localhost/fetchdata, type:, media:, title:link-C, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/custom-C.css, type:, media:print, title:, sizes:",
                "rel:stylesheet, href:_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:"
            );
        }

        [Theory(DisplayName = "Add link elements only on Server")]
        [MemberData(nameof(HostingModels))]
        public async Task AddLinkElementsOnly_on_Server_Test(HostingModel hostingModel)
        {
            _TestContext.StartHost(hostingModel);
            var httpClient = new HttpClient();
            var urlOfCanonical = _TestContext.GetHostUrl(hostingModel).TrimEnd('/') + "/canonical";

            var contentOfCanonical = await httpClient.GetStringAsync(urlOfCanonical);

            DumpLinkElements(contentOfCanonical).Any(
                elemnt => elemnt == $"rel:canonical, href:{urlOfCanonical}, type:, media:, title:, sizes:"
            ).IsTrue();
        }

        private IEnumerable<string> DumpLinkElements(string content)
        {
            return Regex.Matches(content, @"<link[ \t]+[^>]*>")
                .Select(m => (
                    Rel: Regex.Match(m.Value, "rel=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Href: Regex.Match(m.Value, "href=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Type: Regex.Match(m.Value, "type=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Media: Regex.Match(m.Value, "media=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Title: Regex.Match(m.Value, "title=\"(?<t>[^\"]+)\"").Groups["t"].Value,
                    Sizes: Regex.Match(m.Value, "sizes=\"(?<t>[^\"]+)\"").Groups["t"].Value
                ))
                .Where(m => !m.Href.StartsWith("data:text/css,.loading%7B")) // exclude style for "Loading..." element.
                .OrderBy(m => m.Rel).ThenBy(m => m.Href)
                .Select(m => $"rel:{m.Rel}, href:{m.Href}, type:{m.Type}, media:{m.Media}, title:{m.Title}, sizes:{m.Sizes}")
                .ToArray();
        }
    }
}
