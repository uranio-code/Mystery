using Mystery.Content;
using Mystery.Register;
using Mystery.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Mystery.Json
{
    /// <summary>
    /// json converter for reference in the BLL it will write/read the uid only
    /// </summary>
    public class ReferencePropertyValueJsonBLLConverter : JsonConverter
    {

        
        private IDictionary<Type, bool> _can_do = new Dictionary<Type, bool>();

        public ReferencePropertyValueJsonBLLConverter()
        {
            
        }

        private JsonSerializer _serializer = new JsonSerializer();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //we don't write empty references
            if (value == null || ((IContentReference)value).isNullOrEmpty()) {
                writer.WriteNull();
                return;
            }


            JObject jo = JObject.FromObject(value, _serializer);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IContentReference result = (IContentReference)this.getGlobalObject<FastActivator>().createInstance(objectType);
            if (reader.TokenType == JsonToken.Null)
            {
                return result;
            }

            // Load JObject from stream
            JObject jo = JObject.Load(reader);
            JToken guid_token = jo.GetValue(nameof(BaseContent.guid));
            if (guid_token == null || guid_token.Type == JTokenType.Null)
                return result;
            Guid guid = Guid.Empty;
            if (Guid.TryParse(guid_token.Value<string>(), out guid))
                result.guid = guid;

            return result;
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        private bool ComputeCanConvert(Type objectType) {
            if (!objectType.IsGenericType)
                return false;
            return objectType.GetGenericTypeDefinition().Equals(typeof(ContentReference<>));
        }

        private static object _lock = new object();

        public override bool CanConvert(Type objectType)
        {
            if (_can_do.ContainsKey(objectType))
                return _can_do[objectType];
            lock (_lock) {
                _can_do[objectType] = ComputeCanConvert(objectType);
            }
            return _can_do[objectType];
        }
    }
}
