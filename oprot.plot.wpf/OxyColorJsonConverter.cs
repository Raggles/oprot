using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OxyPlot;
using System;
using System.Linq;

namespace oprot.plot.core
{
    public class OxyColorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var name = (OxyColor)value;
            writer.WriteStartObject();
            writer.WritePropertyName("Color");
            serializer.Serialize(writer, name.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            var properties = jsonObject.Properties().ToList();
            return OxyColor.Parse((string)properties[0].Value);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(OxyColor);
        }
    }
}
