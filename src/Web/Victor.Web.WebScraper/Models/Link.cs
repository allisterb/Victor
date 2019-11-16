using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace Victor
{
    public class Link
    {
        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> HtmlAttributes { get; set; }

        [JsonIgnore]
        public string HtmlClass => HtmlAttributes != null && HtmlAttributes.ContainsKey("class") ? HtmlAttributes["class"] : null;

        [JsonIgnore]
        public string InnerHtml { get; set; }

        [JsonIgnore]
        public string InnerText { get; set; }
    }
}
