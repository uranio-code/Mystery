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
    /// json for reference property in the ui, it will add base info
    /// </summary>
    public class ReferencePropertyValueJsonUiConverter : JsonConverter
    {

        private JsonSerializer _serializer = new JsonSerializer();

        private IDictionary<Type, bool> _can_do = new Dictionary<Type, bool>();

        public ReferencePropertyValueJsonUiConverter()
        {
            
        }

        /// <summary>
        /// write content basic info, not its properties
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static JObject getJsonObject(IContent c)
        {
            if (c == null)
                return null;
            var result = new JObject();
            result.Add(nameof(IContent.guid), c.guid);
            var type = c.GetType();
            var ct = type.getMysteryAttribute<ContentType>();
            result.Add(nameof(ContentType), ct.name);
            result.Add(nameof(Type), type.FullName);
            result.Add("tiny_guid", c.guid.Tiny());
            result.Add("data_url", "ContentService/ContentView/" + c.getContenTypeName()+"/" + c.guid.Tiny());
            if (type.getMysteryAttribute<ContentView>() != null)
                result.Add("url", ct.name + "/" + c.guid.Tiny());
            var rt = c.ReferenceText;
            if (!string.IsNullOrEmpty(rt))
                result.Add(nameof(ReferenceText), rt);
            if (c is IContentWithUid)
                result.Add(nameof(IContentWithUid.uid), ((IContentWithUid)c).uid);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //we don't write empty references
            if (value == null || ((IContentReference)value).isNullOrEmpty()) {
                writer.WriteNull();
                return;
            }
                

            var reference = (IContentReference)value;

            //if we reach here it means we have a guid in the reference, 
            //but for some reason we might not be able to reach the content (the content not yet in the common storage)
            //if that is the case we write only the guid
            JObject jo = JObject.FromObject(value, _serializer);
            var c = reference.getContent();
            //if we have the content we add the basic info
            jo.Merge(getJsonObject(c));
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
            if (guid_token==null || guid_token.Type == JTokenType.Null)
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
