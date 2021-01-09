namespace HeadElement.E2ETest
{
    internal static class ExpectLinks
    {
        public static readonly string[] AtHome = new[]{
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:, as:",
                "rel:preload, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:style",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 600px), title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(min-width: 601px), title:, sizes:, as:image",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:"
            };
        public static readonly string[] AtCounter = new[]{
                "rel:canonical, href:/counter, type:, media:, title:link-B, sizes:, as:",
                "rel:icon, href:/_content/SampleSite.Components/favicons/counter-0.png, type:image/png, media:, title:, sizes:, as:",
                "rel:preload, href:/_content/SampleSite.Components/css/blue.png, type:, media:(min-width: 301px), title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/green.png, type:, media:, title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 600px), title:, sizes:, as:image",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:"
            };
        public static readonly string[] AtFetchData = new[]{
                "rel:canonical, href:/fetchdata, type:, media:, title:link-C, sizes:, as:",
                "rel:preload, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:style",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/purple.png, type:, media:, title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 599px), title:, sizes:, as:image",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(min-width: 601px), title:, sizes:, as:image",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-C.css, type:, media:print, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:"
            };
    }
}
