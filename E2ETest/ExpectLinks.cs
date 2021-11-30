namespace HeadElement.E2ETest
{
    internal static class ExpectLinks
    {
        public static readonly string[] AtHome = new[]{
                "rel:alternate, href:/en, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:x-default, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/en/home, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:en, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/ja/home, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:ja, imagesizes:, imagesrcset:, disabled:false",
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:style, crossorigin:anonymous, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 600px), title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:32px, imagesrcset:red.png, 32px, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(min-width: 601px), title:, sizes:, as:image, crossorigin:, hreflang:en-US, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false"
            };
        public static readonly string[] AtCounter = new[]{
                "rel:alternate, href:/en/counter, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:en, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/en/counter, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:x-default, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/ja/counter, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:ja, imagesizes:, imagesrcset:, disabled:false",
                "rel:canonical, href:/counter, type:, media:, title:link-B, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:icon, href:/_content/SampleSite.Components/favicons/counter-0.png, type:image/png, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/blue.png, type:, media:(min-width: 301px), title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:32px, imagesrcset:blue.png, 32px, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/green.png, type:, media:, title:, sizes:, as:image, crossorigin:anonymous, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 600px), title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:32px, imagesrcset:red.png, 32px, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false"
            };
        public static readonly string[] AtFetchData = new[]{
                "rel:alternate, href:/en, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:x-default, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/en/fetchdata, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:en, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/fr/fetchdata, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:fr, imagesizes:, imagesrcset:, disabled:false",
                "rel:canonical, href:/fetchdata, type:, media:, title:link-C, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:style, crossorigin:anonymous, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/purple.png, type:, media:, title:, sizes:, as:image, crossorigin:, hreflang:en, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 599px), title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:48px, imagesrcset:red.png, 48px, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(min-width: 601px), title:, sizes:, as:image, crossorigin:, hreflang:en-US, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:custom, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-C.css, type:, media:print, title:, sizes:, as:, crossorigin:anonymous, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false"
            };
        public static readonly string[] AtOnAfterRender = new[]{
                "rel:alternate, href:/en, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:x-default, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/en/home, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:en, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/ja/home, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:ja, imagesizes:, imagesrcset:, disabled:false",
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 600px), title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:32px, imagesrcset:red.png, 32px, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:2nd title, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false"
            };
        public static readonly string[] AtOnAfterRenderPrerendered = new[]{
                "rel:alternate, href:/en, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:x-default, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/en/home, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:en, imagesizes:, imagesrcset:, disabled:false",
                "rel:alternate, href:/ja/home, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:ja, imagesizes:, imagesrcset:, disabled:false",
                "rel:icon, href:/_content/SampleSite.Components/favicons/favicon.ico, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:style, crossorigin:anonymous, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/orange.png, type:, media:, title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(max-width: 600px), title:, sizes:, as:image, crossorigin:, hreflang:, imagesizes:32px, imagesrcset:red.png, 32px, disabled:false",
                "rel:preload, href:/_content/SampleSite.Components/css/red.png, type:, media:(min-width: 601px), title:, sizes:, as:image, crossorigin:, hreflang:en-US, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/bootstrap/bootstrap.min.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-A.css, type:, media:, title:1st title, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/custom-X.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false",
                "rel:stylesheet, href:/_content/SampleSite.Components/css/site.css, type:, media:, title:, sizes:, as:, crossorigin:, hreflang:, imagesizes:, imagesrcset:, disabled:false"
            };
    }
}
