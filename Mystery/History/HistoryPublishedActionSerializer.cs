using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Mystery.MysteryAction;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.History
{

    public class HistoryPublishedAction : IPublishedAction<Dictionary<string, object>>
    {
        public Dictionary<string,object> history_message_data { get; set; }

        public string history_message_template_url { get; set; }

        public List<string> history_tags { get; set; }
    }

    class HistoryPublishedActionSerializer : IBsonSerializer
    {
        public Type ValueType
        {
            get
            {
                return typeof(IPublishedAction);
            }
        }

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return BsonSerializer.Deserialize<HistoryPublishedAction>(context.Reader);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            throw new NotImplementedException();
        }
    }

    class HistoryPublishedActionsSerializer : IBsonSerializer
    {
        public Type ValueType
        {
            get
            {
                return typeof(List<IPublishedAction>);
            }
        }

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            //we deserialize using HistoryPublishedAction
            var result = BsonSerializer.Deserialize<List<HistoryPublishedAction>>(context.Reader);
            if (result == null)
                result = new List<HistoryPublishedAction>();
            return new List<IPublishedAction>(from x in result select x);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            BsonSerializer.Serialize(context.Writer, typeof(List<IPublishedAction>), value, args: args);
        }
    }
}
