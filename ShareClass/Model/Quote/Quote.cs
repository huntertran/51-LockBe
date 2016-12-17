using Newtonsoft.Json;

namespace ShareClass.Model.Quote
{
    public class Quote
    {
        [JsonProperty("quote")]
        public string QuoteString { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }
    }

}
