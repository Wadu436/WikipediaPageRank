using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WikipediaREST
{
    //Generated using QuickType

    public partial class WikipediaPropLinks
    {
        [JsonProperty("batchcomplete")]
        public string Batchcomplete { get; set; }

        [JsonProperty("continue")]
        public Continue Continue { get; set; }

        [JsonProperty("query")]
        public Query Query { get; set; }

        [JsonProperty("limits")]
        public Limits Limits { get; set; }
    }

    public partial class Continue
    {
        [JsonProperty("plcontinue")]
        public string Plcontinue { get; set; }

        [JsonProperty("continue")]
        public string ContinueContinue { get; set; }
    }

    public partial class Limits
    {
        [JsonProperty("links")]
        public int Links { get; set; }
    }

    public partial class Query
    {
        [JsonProperty("pages")]
        public Dictionary<string, Page> Pages { get; set; }
    }

    public partial class Page
    {
        [JsonProperty("pageid")]
        public int Pageid { get; set; }

        [JsonProperty("ns")]
        public int Ns { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }
    }

    public partial class Link
    {
        [JsonProperty("ns")]
        public long Ns { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    public partial class WikipediaPropLinks
    {
        public static WikipediaPropLinks FromJson(string json) => JsonConvert.DeserializeObject<WikipediaPropLinks>(json, WikipediaREST.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this WikipediaPropLinks self) => JsonConvert.SerializeObject(self, WikipediaREST.Converter.Settings);
    }

    internal class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
