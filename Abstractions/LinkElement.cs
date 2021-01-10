using System.Text.Json.Serialization;

namespace Toolbelt.Blazor.HeadElement
{
    public class LinkElement
    {
        [JsonPropertyName("r")]
        public string Rel { get; set; } = "";

        [JsonPropertyName("h")]
        public string Href { get; set; } = "";

        [JsonPropertyName("s")]
        public string Sizes { get; set; } = "";

        [JsonPropertyName("p")]
        public string Type { get; set; } = "";

        [JsonPropertyName("t")]
        public string Title { get; set; } = "";

        [JsonPropertyName("m")]
        public string Media { get; set; } = "";

        [JsonPropertyName("a")]
        public string As { get; set; } = "";

        [JsonPropertyName("co")]
        public string CrossOrigin { get; set; } = "";//:string null

        [JsonPropertyName("hl")]
        public string Hreflang { get; set; } = "";//:string ""

        [JsonPropertyName("isz")]
        public string ImageSizes { get; set; } = "";//:string ""

        [JsonPropertyName("iss")]
        public string ImageSrcset { get; set; } = "";//:string ""

        [JsonPropertyName("d")]
        public bool Disabled { get; set; }//:bool

        public LinkElement()
        {
        }

        public LinkElement(string rel, string href, string sizes, string type, string title, string media)
        {
            Rel = rel;
            Href = href;
            Sizes = sizes ?? "";
            Type = type ?? "";
            Title = title ?? "";
            Media = media ?? "";
        }

        public LinkElement(string rel, string href, string sizes, string type, string title, string media, string @as)
        {
            Rel = rel;
            Href = href;
            Sizes = sizes ?? "";
            Type = type ?? "";
            Title = title ?? "";
            Media = media ?? "";
            As = @as ?? "";
        }

        public LinkElement(string rel, string href, string sizes = "", string type = "", string title = "", string media = "", string @as = "", string crossOrigin = "", string hreflang = "", string imageSizes = "", string imageSrcset = "", bool disabled = false)
        {
            Rel = rel;
            Href = href;
            Sizes = sizes ?? "";
            Type = type ?? "";
            Title = title ?? "";
            Media = media ?? "";
            As = @as ?? "";
            CrossOrigin = crossOrigin ?? "";
            Hreflang = hreflang ?? "";
            ImageSizes = imageSizes ?? "";
            ImageSrcset = imageSrcset ?? "";
            Disabled = disabled;
        }
    }
}
