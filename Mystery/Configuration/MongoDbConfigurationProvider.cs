using MongoDB.Bson;
using MongoDB.Driver;
using Mystery.Json;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.Configuration
{
    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IConfigurationProvider), overrides_exsisting = false, singleton = true)]
    public class MongoDbConfigurationProvider : IConfigurationProvider
    {

        private Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        private static object _lock = new object();


        public static string collectionName = "Configurations";

        public static string default_env_name = "theNormalEnv";

        public string env_name { get; set; } = default_env_name;

        IMongoCollection<BsonDocument> collection;

        private void InstanceClient()
        {
            // Create a new instance of the DocumentClient
            collection = this.getGlobalObject<MysteryMongoDb>().local_db.GetCollection<BsonDocument>(collectionName);
        }

        public MongoDbConfigurationProvider() {
            InstanceClient();
        }


        public T getConfiguration<T>() where T : new()
        {
            Type type = typeof(T);
            if (_instances.ContainsKey(type))
                return (T)_instances[type];
            lock (_lock)
            {
                if (_instances.ContainsKey(type))
                    return (T)_instances[type];
                var builder = Builders<BsonDocument>.Filter;
                var filter = builder.Eq(nameof(Type), typeof(T).Name) &
                    builder.Eq(nameof(env_name), env_name);
                BsonDocument doc = collection.FindSync(filter).FirstOrDefault();
                T result = default(T);
                if (doc == null)
                {
                    result = new T();
                    this.setConfiguration<T>(result);
                }
                else
                {
                    doc.Remove("_id");
                    result = this.getGlobalObject<MysteryJsonConverter>().readJson<T>(doc.ToJson());
                }

                _instances[type] = result;
                return result;
            }
        }

        public void setConfiguration<T>(T conf)
        {
            string json = this.getGlobalObject<MysteryJsonConverter>().getJson(conf);
            BsonDocument doc = BsonDocument.Parse(json);
            var update = new BsonDocument();
            doc.Add(nameof(Type), conf.GetType().Name);
            doc.Add(nameof(env_name), env_name);
            update.Add("$set", doc);
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq(nameof(Type), conf.GetType().Name) &
                builder.Eq(nameof(env_name), env_name);
            collection.UpdateOne(filter, update, new UpdateOptions() { IsUpsert=true});
            lock (_lock)
            {
                _instances[typeof(T)] = conf;
            }
        }
        public void flush()
        {
            lock (_lock)
            {
                _instances = new Dictionary<Type, object>();
            }
        }
    }
}
