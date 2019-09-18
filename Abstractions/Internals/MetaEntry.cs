using System.Text.Json.Serialization;

namespace Toolbelt.Blazor.HeadElement.Internals
{
    public class MetaEntry
    {
        [JsonPropertyName("k")]
        public string Key { get; set; }

        [JsonPropertyName("t")]
        public MetaEntryKeyType KeyType { get; set; }

        [JsonPropertyName("c")]
        public string Content { get; set; }
    }
}
