using Mystery.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Mystery.Register;
using Mystery.UI;
using System.ComponentModel;
using Newtonsoft.Json.Converters;

namespace Mystery.Json
{
    public class ContentConverter : JsonConverter
    {

        private Guid _guid = Guid.NewGuid();

        JsonSerializer _serializer = new JsonSerializer();

        public ContentConverter() {
            _serializer.Converters.Add(new ReferencePropertyValueJsonBLLConverter());
            _serializer.Converters.Add(new DatePropertyValueJsonConverter());
            _serializer.Converters.Add(new StringEnumConverter());
            _serializer.ContractResolver = new WritablePropertiesOnlyResolver();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(value, _serializer);
            var c = (IContent)value;
            jo.Merge(ReferencePropertyValueJsonUiConverter.getJsonObject(c));
            jo.WriteTo(writer);
        }

        private Type readTypeByClassName(JObject jo) {
            JToken type_token = jo.GetValue("ClassName");
            if (type_token == null)
                return null;
            if (type_token.Type == JTokenType.Null)
                return null;
            Type type = this.getMystery().AssemblyRegister.getTypeByFullName(type_token.Value<string>());
            if (type == null)
                return null;
            if (!typeof(IContent).IsAssignableFrom(type))
                return null;
            //good!
            return type;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IContent result=null;

            if (reader.TokenType == JsonToken.Null)
                return result;

            // Load JObject from stream
            JObject jo = JObject.Load(reader);

            var activator = this.getGlobalObject<FastActivator>();

            //looking for the type, we go 3 ways
            //first we try to see if in json there is a ClassName element
            Type type = readTypeByClassName(jo);
            //if we could not find it we go for the content_type attribute
            if(type == null)
                type = readTypeByContentType(jo);
            //last hope is actually the type given
            if (type == null) {
                if (!objectType.IsAbstract &&
                    objectType != typeof(BaseContent) &&
                    typeof(IContent).IsAssignableFrom(objectType) &&
                    objectType.getMysteryAttribute<ContentType>() != null)
                    type = objectType;
            }
            if (type == null)
                throw new Exception("can't detect the content type from the json and " + objectType.FullName + " isn't a valid one");
            result = (IContent)activator.createInstance(type);

            if (result is ISupportInitialize) {
                ((ISupportInitialize)result).BeginInit();
            }

            //Create a new reader for this jObject, and set all properties to match the original reader.
            JsonReader jObjectReader = jo.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            serializer.Populate(jObjectReader, result);

            
            foreach (ContentProperty content_property in result.GetType().getMysteryPropertyAttributes<ContentProperty>())
            {
                if (!typeof(IReferenceProperty).IsAssignableFrom(content_property.used_in.PropertyType))
                    continue; //not a reference
                if (content_property.retrive(result) != null)
                    continue; //instanced already
                content_property.save(result, activator.createInstance(content_property.used_in.PropertyType));
            }

            if (result is ISupportInitialize)
            {
                ((ISupportInitialize)result).EndInit();
            }
            return result;
        }

        private Type readTypeByContentType(JObject jo)
        {
            JToken ct_token = jo.GetValue(nameof(ContentType));

            //ContentType is mandatory property
            if (ct_token == null || ct_token.Type == JTokenType.Null)
            {
                return null;
            }

            string ct_name = ct_token.Value<string>();
            if (string.IsNullOrEmpty(ct_name) || ContentType.getType(ct_name) == null)
                throw new Exception("unrecognized " + nameof(ContentType) + ": " + ct_name);
            Type type = ContentType.getType(ct_name);
            return type;
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IContent).IsAssignableFrom(objectType);
        }
    }

    class ContentsConverter : JsonConverter
    {

        private JsonSerializer _serializer = new JsonSerializer();
        
        public ContentsConverter()
        {
            _serializer.Converters.Add(new ContentConverter());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<IContent> contents = (IEnumerable < IContent > )value;
            JArray ja = JArray.FromObject(contents, _serializer);
            ja.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<IContent> result = new List<IContent>();

            if (reader.TokenType == JsonToken.Null)
                return result;

            JArray ja = JArray.Load(reader);

            //Create a new reader for this jObject, and set all properties to match the original reader.
            JsonReader jObjectReader = ja.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            _serializer.Populate(jObjectReader, result);

            return result;
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IEnumerable<IContent>).IsAssignableFrom(objectType);
        }
    }

    class ContentDictConverter : JsonConverter
    {

        private JsonSerializer _serializer = new JsonSerializer();

        public ContentDictConverter()
        {
            _serializer.Converters.Add(new ContentConverter());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = JObject.FromObject(value, _serializer);
            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Dictionary<Guid,IContent> result = new Dictionary<Guid, IContent>();

            if (reader.TokenType == JsonToken.Null)
                return result;

            JObject jo = JObject.Load(reader);

            //Create a new reader for this jObject, and set all properties to match the original reader.
            JsonReader jObjectReader = jo.CreateReader();
            jObjectReader.Culture = reader.Culture;
            jObjectReader.DateParseHandling = reader.DateParseHandling;
            jObjectReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jObjectReader.FloatParseHandling = reader.FloatParseHandling;

            // Populate the object properties
            _serializer.Populate(jObjectReader, result);

            return result;
        }

        public override bool CanRead
        {
            get { return base.CanRead; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary<Guid,IContent>).IsAssignableFrom(objectType);
        }
    }
}
