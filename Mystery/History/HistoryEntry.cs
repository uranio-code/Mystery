using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using Mystery.Content;
using Mystery.Json;
using Mystery.MysteryAction;
using Mystery.Users;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.History
{


    public class HistoryEntry
    {

        [BsonId]
        public Guid id { get; set; } 

        public DateTime date { get; set; }

        [BsonSerializer(typeof(HistoryPublishedActionsSerializer))]
        public List<IPublishedAction> logs { get; set; }

        [JsonConverter(typeof(HistoryUserJsonConverter))]
        public User autheticated_user { get; set; }

        [JsonConverter(typeof(HistoryUserJsonConverter))]
        public User working_for { get; set; }

        [JsonConverter(typeof(ContentsConverter))]
        public List<IContent> Added { get; set; } = new List<IContent>();

        [JsonConverter(typeof(ContentsConverter))]
        public List<IContent> Removed { get; set; } = new List<IContent>();

        [JsonConverter(typeof(ContentDictConverter))]
        public Dictionary<string,IContent> PreviousValues { get; set; } = new Dictionary<string, IContent>();

        [JsonConverter(typeof(ContentDictConverter))]
        public Dictionary<string, IContent> NewValues { get; set; } = new Dictionary<string, IContent>();

        [BsonElement]
        public List<Guid> guids { get {
                return new List<Guid>(
                    (from x in Added select x.guid)
                    .Union(from x in Removed select x.guid)
                    .Union(from x in NewValues.Keys select Guid.Parse(x)));
            } }

        [JsonIgnore()]
        public bool IsEmpty { get {
                return Added.Count == 0 && Removed.Count == 0 && PreviousValues.Count == 0 && (logs == null || logs.Count == 0);
            }}
    }
}
