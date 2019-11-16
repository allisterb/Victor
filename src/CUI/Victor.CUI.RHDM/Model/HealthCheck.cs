namespace Victor.CUI.RHDM.KIE.Model
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class HealthCheck
    {
        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("timestamp")]
        public HealthCheckTimestamp Timestamp { get; set; }

        [JsonProperty("content")]
        public string[] Content { get; set; }
    }

    public partial class HealthCheckTimestamp
    {
        [JsonProperty("java.util.Date")]
        public long JavaUtilDate { get; set; }
    }
}
