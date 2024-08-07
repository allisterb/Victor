﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Victor;
//
//    var response = Response.FromJson(jsonString);

namespace Victor
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SnipsIntentEntity
    {
        [JsonProperty("rawValue")]
        public string RawValue { get; set; }

        [JsonProperty("value")]
        public SnipsIntentEntityValue Value { get; set; }

        [JsonProperty("alternatives")]
        public object[] Alternatives { get; set; }

        [JsonProperty("range")]
        public SnipsIntentEntityRange Range { get; set; }

        [JsonProperty("entity")]
        public string Entity { get; set; }

        [JsonProperty("slotName")]
        public string SlotName { get; set; }
    }

    public partial class SnipsIntentEntityRange
    {
        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }
    }

    public partial class SnipsIntentEntityValue
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("value")]
        public string ValueValue { get; set; }
    }

    public partial class SnipsIntentEntity
    {
        public static SnipsIntentEntity[] FromJson(string json) => JsonConvert.DeserializeObject<SnipsIntentEntity[]>(json, Victor.SnipsIntentEntityConverter.Settings);
    }

    public static class SnipsIntentEntitySerialize
    {
        public static string ToJson(this SnipsIntentEntity[] self) => JsonConvert.SerializeObject(self, Victor.SnipsIntentEntityConverter.Settings);
    }

    internal static class SnipsIntentEntityConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
