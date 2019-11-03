using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace Victor
{
    public class MillisecondEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalMilliseconds + "000");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return _epoch.AddMilliseconds((long)reader.Value);
        }

    }

    public partial class EDDIClient
    {
        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, new MillisecondEpochConverter());

        public static string Serialize<T>(T value) => JsonConvert.SerializeObject(value, new MillisecondEpochConverter());

    }
}
