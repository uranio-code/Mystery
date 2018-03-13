using MongoDB.Bson.Serialization.Attributes;
using Mystery.Content;
using Mystery.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Messaging
{
    public interface IUserMessage
    {
        Guid guid { get;}
        string title { get; set; }
        string body { get; set; }
        User from { get; set; }
        IEnumerable<User> to { get; set; }
        IEnumerable<User> cc { get; set; }
        DateTime date { get; }
    }

    public class BaseMessage : IUserMessage
    {
        [BsonId]
        public Guid guid { get; private set; } = Guid.NewGuid();
        public string title { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public User  from { get; set; }
        public IEnumerable<User> to { get; set; } = new List<User>();
        public IEnumerable<User> cc { get; set; } = new List<User>();

        public DateTime date { get; set; } = DateTime.Now;
    }
}
