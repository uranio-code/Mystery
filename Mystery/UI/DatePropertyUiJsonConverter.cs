using Mystery.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mystery.Json
{
    class DatePropertyUiJsonConverter : JsonConverter
    {

        private class helper {
            public string ddate { get; set; } = nameof(DateTime);
            public DateTime value { get; set; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(new helper() { value = (DateTime)value });
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime result = DateTime.MinValue;
            if (reader.TokenType == JsonToken.Null)
                return result;
            if (reader.Value is DateTime)
            {
                result = (DateTime)reader.Value;
            }
            else if (reader.Value is string)
            {
                result = DateTime.Parse((string)reader.Value);
            }
            else if (reader.TokenType == JsonToken.StartObject) {
                JObject jo = JObject.Load(reader);
                result = jo[nameof(helper.value)].Value<DateTime>();
            }
            else if (!(reader.Value == null))
            {
                throw new NotImplementedException(reader.Value.GetType().Name + " doesn't have an implementation for dates");
            }
            return result;
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType);
        }

    }

}
