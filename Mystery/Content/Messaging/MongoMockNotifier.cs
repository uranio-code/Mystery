using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using MongoDB.Driver;
using Mystery.Json;
using Mystery.Users;

namespace Mystery.Messaging
{
    /// <summary>
    /// this implementation store the message in the local mongo
    /// it is the first implementation to be able to see the sent notification
    /// </summary>
    [GlobalAvalibleObjectImplementation(
        implementation_of = typeof(INotifier),
        overrides_exsisting = false,
        singleton = true)]
    public class MongoMockNotifier : INotifier
    {

        private IMongoCollection<IUserMessage> _collection;

        public MongoMockNotifier() {
            _collection = this.getGlobalObject<MysteryMongoDb>()
                .local_db.GetCollection<IUserMessage>(nameof(IUserMessage));
        }

        public void sendMessage(IUserMessage message)
        {
            if (message == null)
                return;
            if (message.from != null && message.from.account_type == UserType.normal)
            {
                //TBD translation
                message.body = "message from: " + message.from.ReferenceText + "<br/>" + Environment.NewLine+message.body;
            }
            _collection.InsertOne(message);
        }
    }
}
