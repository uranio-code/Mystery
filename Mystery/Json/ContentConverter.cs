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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            IContent result=null;

            if (reader.TokenType == JsonToken.Null)
                return result;

            // Load JObject from stream
            JObject jo = JObject.Load(reader);
            JToken ct_token = jo.GetValue(nameof(ContentType));

            //ContentType is mandatory property
            if (ct_token == null || ct_token.Type == JTokenType.Null)
            {
                //this converter doesn't work with recursion
                if (!string.IsNullOrEmpty(reader.Path))
                    return null;
                throw new Exception("invalid json for content, " + nameof(ContentType) + " property missing");
            }
                

            //if we know the Type than even better
            JToken type_token = jo.GetValue("ClassName");
            if (type_token != null && type_token.Type != JTokenType.Null)
            {
                Type type = this.getMystery().AssemblyRegister.getTypeByFullName(type_token.Value<string>());
                if (type != null && typeof(IContent).IsAssignableFrom(type))
                    result = (IContent)this.getGlobalObject<FastActivator>().createInstance(type);
            }

            var activator = this.getGlobalObject<FastActivator>();
            //otherwise we use the ContentType
            if (result == null) {

                
                string ct_name = ct_token.Value<string>();
                if (string.IsNullOrEmpty(ct_name) || ContentType.getType(ct_name) == null)
                    throw new Exception("unrecognized " + nameof(ContentType) + ": " + ct_name);
                Type type = ContentType.getType(ct_name);
                result = (IContent)activator.createInstance(type);
            }

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
