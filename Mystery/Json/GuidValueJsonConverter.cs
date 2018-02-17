using Mystery.Content;
using Newtonsoft.Json;
using System;

namespace Mystery.Json
{
    /// <summary>
    /// allow also the base64 tiny representation
    /// </summary>
    class GuidValueJsonConverter : JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            
            if (reader.Value == null)
                return Guid.Empty;

            if (reader.Value is string)
            {
                string value = (string)reader.Value;
                Guid parsed = value.fromTiny();
                if (parsed == Guid.Empty && !Guid.TryParse(value, out parsed))
                    return Guid.Empty;
                return parsed;
            }
            else if(!(reader.Value == null))
            {
                throw new NotImplementedException(reader.Value.GetType().Name + " doesn't have an implementation for Guid");
            }
            return Guid.Empty;
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.Equals(typeof(Guid));
        }

    }

}
