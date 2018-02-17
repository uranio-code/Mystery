using MongoDB.Bson;
using MongoDB.Driver;
using Mystery.Json;
using Mystery.Register;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mystery.MysteryAction;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Mystery.Users;
using Mystery.Content;

namespace Mystery.History
{
    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IHistoryRepository), overrides_exsisting = true, singleton = true)]
    public class MongoDBHistoryRepository : IHistoryRepository
    {

        public static string collectionName { get; private set; } = "History";

        IMongoCollection<HistoryEntry> collection;

        private void InstanceClient()
        {
            // Create a new instance of the DocumentClient
            collection = this.getGlobalObject<MysteryMongoDb>().content_db.GetCollection<HistoryEntry>(collectionName);
        }
        public MongoDBHistoryRepository() {
            InstanceClient();
        }
        /// <summary>
        /// CAUTION: this will remove the whole history in db
        /// </summary>
        public void DropAllHistory() {
            this.collection.DeleteMany(x => true);
        }

        public async Task AddAsync(HistoryEntry he)
        {
            await collection.InsertOneAsync(he);
        }

        public void Add(HistoryEntry he)
        {
            collection.InsertOne(he);
        }

        public IEnumerable<HistoryEntry> GetByFilter(Expression<Func<HistoryEntry, bool>> filter)
        {
            return collection.Find(filter).ToEnumerable();
        }

        public class UnwindedHistoryEntry
        {
            public Guid id { get; set; }

            public DateTime date { get; set; }

            [BsonSerializer(typeof(HistoryPublishedActionSerializer))]
            public IPublishedAction  logs { get; set; }

        }


        public IEnumerable<IPublishedAction> GetByTags(
            IEnumerable<string> tags, int max_result = 20, DateTime min_date = default(DateTime))
        {
            var unwind = collection.Aggregate()
                .Match(x => x.date > min_date && x.logs != null && x.logs.Count > 0)
                .SortByDescending(x => x.date)
                .Unwind<HistoryEntry, UnwindedHistoryEntry>(x => x.logs);

            ICollection<IPublishedAction> result = new LinkedList<IPublishedAction>();
            using (var enumerator = unwind.ToCursor())
            {
                while (enumerator.MoveNext())
                {
                    foreach (var entry in enumerator.Current) {
                        if (max_result > 0 && result.Count >= max_result)
                            return result;
                        if (entry.logs.history_tags.Intersect(tags).FirstOrDefault() == null)
                            continue;
                        result.Add(entry.logs);
                    }
                }
            }
            
              
            return result;
                
        }

        public IEnumerable<IPublishedAction> GetByUser(
            User user, int max_result = 0, DateTime min_date = default(DateTime))
        {
            if (user == null)
                return new LinkedList<IPublishedAction>();
            var unwind = collection.Find(
                x=>x.autheticated_user.guid == user.guid 
                &&
                x.date > min_date).SortByDescending(x => x.date).Limit(max_result);

            ICollection<IPublishedAction> result = new LinkedList<IPublishedAction>();
            using (var enumerator = unwind.ToCursor())
            {
                while (enumerator.MoveNext())
                {
                    foreach (var entry in enumerator.Current)
                    {
                        if (max_result > 0 && result.Count >= max_result)
                            return result;
                        result.AddRange(entry.logs);
                    }
                }
            }
            return result;
        }

        public IEnumerable<IPublishedAction> GetByContent(
            IContent content, int max_result = 0, DateTime min_date = default(DateTime))
        {
            if (content == null)
                return new LinkedList<IPublishedAction>();
            var unwind = collection.Find(
                x => x.guids.Contains(content.guid)
                &&
                x.date > min_date).SortByDescending(x => x.date).Limit(max_result);

            ICollection<IPublishedAction> result = new LinkedList<IPublishedAction>();
            using (var enumerator = unwind.ToCursor())
            {
                while (enumerator.MoveNext())
                {
                    foreach (var entry in enumerator.Current)
                    {
                        if (max_result > 0 && result.Count >= max_result)
                            return result;
                        result.AddRange(entry.logs);
                    }
                }
            }
            return result;
        }
    }
}
