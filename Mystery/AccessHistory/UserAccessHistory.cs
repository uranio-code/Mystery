using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using MongoDB.Driver;
using Mystery.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Mystery.AccessHistory
{

    public class AccessRecord {
        [BsonId]
        public Guid record_guid { get; set; } = Guid.NewGuid();

        public Guid content_guid { get; set; } = Guid.NewGuid();

        public string content_type_name { get; set; }

        public string session_id { get; set; }

        public DateTime entry_date { get; set; }

        public Guid user_guid { get; set; } = Guid.NewGuid();
    }

    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class UserAccessHistory
    {
        
        
        public IMongoCollection<AccessRecord> collection { get; private set; }

        public UserAccessHistory() {
            var db = this.getGlobalObject<MysteryMongoDb>()
                    .content_db;
            
            try
            {
                //100 Mb max
                var options = new CreateCollectionOptions { Capped = true, MaxSize = 100 * 1024 * 1024 };
                db.CreateCollection(nameof(UserAccessHistory), options);
            }
            catch (MongoCommandException ex) {
                if (ex.Code != 48) //already existing
                    throw;
            }
            collection = db.GetCollection<AccessRecord>(nameof(UserAccessHistory));

        }
    }
}
