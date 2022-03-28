using System.Text.Json.Serialization;

namespace Toolbelt.Blazor.HeadElement
{
    public class MetaElement
    {
        [JsonPropertyName("p")]
        public string Property { get; set; } = "";

        [JsonPropertyName("h")]
        public string HttpEquiv { get; set; } = "";

        [JsonPropertyName("n")]
        public string Name { get; set; } = "";

        [JsonPropertyName("m")]
        public string Media { get; set; } = "";

        [JsonPropertyName("c")]
        public string Content { get; set; } = "";

        public static MetaElement ByProp(string property, string content = "") => new MetaElement { Property = property, Content = content };

        public static MetaElement ByProp(string property, string content, string media = "") => new MetaElement { Property = property, Content = content, Media = media };

        public static MetaElement ByName(string name, string content = "") => new MetaElement { Name = name, Content = content };

        public static MetaElement ByName(string name, string content, string media = "") => new MetaElement { Name = name, Content = content, Media = media };

        public static MetaElement ByHttpEquiv(string httpEquiv, string content = "") => new MetaElement { HttpEquiv = httpEquiv, Content = content };

        public static MetaElement ByHttpEquiv(string httpEquiv, string content, string media) => new MetaElement { HttpEquiv = httpEquiv, Content = content, Media = media };
    }
}
