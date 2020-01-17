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

        public LinkElement()
        {
        }

        public LinkElement(string rel, string href, string sizes = "", string type = "", string title = "", string media = "")
        {
            Rel = rel;
            Href = href;
            Sizes = sizes ?? "";
            Type = type ?? "";
            Title = title ?? "";
            Media = media ?? "";
        }
    }
}
