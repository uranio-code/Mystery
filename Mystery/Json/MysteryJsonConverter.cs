﻿using System;
using Mystery.Register;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mystery.Json
{

    [GlobalAvalibleObjectImplementation(singleton = true,implementation_of =typeof(IMysteryJsonConverter))]
    public class MysteryJsonConverter : IMysteryJsonConverter
    {


        private JsonSerializerSettings _setting = new JsonSerializerSettings() { Converters =
            {
                new ContentConverter(),
                new ReferencePropertyValueJsonBLLConverter(),
                new DatePropertyValueJsonConverter(),
                new TypeConverter(),
                new StringEnumConverter(),
                new GuidValueJsonConverter(),
            },
            Formatting = Formatting.Indented,
            ContractResolver = new WritablePropertiesOnlyResolver(),
        };

        public string getJson(object item) {
            
            return Newtonsoft.Json.JsonConvert.SerializeObject(item, _setting);
        }
        
        /// <summary>
        /// read an object from json, if ObjectType is a content or IContent the returning object class
        /// will be automatically selected.
        /// </summary>
        /// <typeparam name="ObjectType"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public ObjectType readJson<ObjectType>(string json) 
        {
            if (string.IsNullOrEmpty(json)) return default(ObjectType);
            return JsonConvert.DeserializeObject<ObjectType>(json,_setting);
        }
        public object readJson(string json,Type objectType)
        {
            if (string.IsNullOrEmpty(json)) return null;
            return JsonConvert.DeserializeObject(json, objectType, _setting);
        }


    }
}
