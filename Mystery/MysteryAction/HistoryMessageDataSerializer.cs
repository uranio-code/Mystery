using System;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;
using Mystery.Register;
using Mystery.UI;
using MongoDB.Bson;
using Mystery.UiJson;

namespace Mystery.MysteryAction
{
    public class HistoryMessageDataSerializer<T> : IBsonSerializer
    {
        public Type ValueType
        {
            get
            {
                return typeof(T);
            }
        }

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            throw new NotImplementedException();
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            //history date is then stream directly to the view
            //for that we must make sure client data is avalible
            var json_converter = this.getGlobalObject<IMysteryJsonUiConverter>();
            var json = json_converter.getJson(value);
            var doc = BsonDocument.Parse(json);
            BsonSerializer.Serialize(context.Writer, doc, args: args);
        }
    }
}