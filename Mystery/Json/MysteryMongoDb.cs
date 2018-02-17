using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Mystery.Configuration;
using Mystery.Register;
using System;

namespace Mystery.Json
{

    public class MongoContentDbConfiguration{
        public string connection_string { get; set; } 
        public string database_name { get; set; } 
    }

    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class MysteryMongoDb
    {

        public MysteryMongoDb() {
            //we apply our conventions

            //enums shall became string
            var pack = new ConventionPack  {
                new EnumRepresentationConvention(BsonType.String),
                new IgnoreExtraElementsConvention(ignoreExtraElements:true),
            };
            ConventionRegistry.Register("MysteryConvetions", pack, t => true);
            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.LocalInstance);
        }

        public IMongoDatabase local_db {
            get
            {
                string connection = Mystery.Properties.Resources.mongoDBConnection;
                if (string.IsNullOrEmpty(connection))
                {
                    return new MongoClient().GetDatabase(nameof(Mystery));
                }
                else
                {
                    return new MongoClient(connection).GetDatabase(nameof(Mystery));
                }
            }
            private set
            {
            }
        }

        private IMongoDatabase _content_db;
        public IMongoDatabase content_db {
            get {
                if (_content_db != null)
                    return _content_db;
                var config = this.getGlobalObject<IConfigurationProvider>()
                .getConfiguration<MongoContentDbConfiguration>();

                IMongoClient client;
                if (string.IsNullOrEmpty(config.connection_string))
                    client = new MongoClient();
                else
                    client = new MongoClient(config.connection_string);

                _content_db = client.GetDatabase(config.database_name);
                return _content_db;
            }
        } 

    }
}
