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
            writer.WriteValue(value);
            //var o = new helper() {value = (DateTime)value };
            //writer.WriteValue(o);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime result = DateTime.MinValue;
            if (reader.Value is DateTime)
            {
                result = (DateTime)reader.Value;
            }
            else if (reader.Value is string)
            {
                result = DateTime.Parse((string)reader.Value);
            }
            else if(!(reader.Value == null))
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
