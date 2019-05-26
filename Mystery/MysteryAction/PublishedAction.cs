using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;

namespace Mystery.MysteryAction
{

    public interface IPublishedAction {
        bool has_history { get; }
        string history_message_template_url { get; }
        List<string> history_tags { get; }
    }

    public interface IPublishedAction<T>: IPublishedAction
    {
        T history_message_data { get; }
    }

    /// <summary>
    /// helper class to proxy action log serilization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PublishedActionLog<T> : IPublishedAction<T>
    {

        static PublishedActionLog() {
            BsonClassMap.RegisterClassMap<PublishedActionLog<T>>(cm => {
                cm.AutoMap();
                cm.GetMemberMap(c => c.history_message_data).SetSerializer(new HistoryMessageDataSerializer<T>());
            });
        }

        public T history_message_data { get; set; }
        public string history_message_template_url { get; set; }

        public List<string> history_tags { get; set; }

        [JsonIgnore]
        public bool has_history { get; set; }
    }

    public static class PublishedActionExtensions
    {

        private abstract class Helper
        {
            public abstract IPublishedAction CreateLog(BaseMysteryAction action);
        }
        private class Helper<T>: Helper
        {
            public override IPublishedAction CreateLog(BaseMysteryAction action) {
                var input = (IPublishedAction<T>)action;
                if (!input.has_history)
                    return null;

                return new PublishedActionLog<T> {
                    history_message_data = input.history_message_data,
                    history_message_template_url = input.history_message_template_url,
                    history_tags = input.history_tags,
                };
            }
        }

        private static IDictionary<Type, Helper> publishing_types = new ConcurrentDictionary<Type, Helper>();

        private static Helper isPublishing(Type t) {
            foreach (Type interface_type in t.GetInterfaces()) {
                if (!interface_type.IsGenericType)
                    continue;
                if (interface_type.GetGenericTypeDefinition() != typeof(IPublishedAction<>))
                    continue;
                return (Helper)Activator.CreateInstance(
                    typeof(Helper<>).MakeGenericType(interface_type.GetGenericArguments().FirstOrDefault())
                    );
            }
            return null;
        }

        public static IPublishedAction getLog(this BaseMysteryAction action)
        {
            if (action == null)
                return null;
            var action_type = action.GetType();
            if (!publishing_types.ContainsKey(action_type)) {
                publishing_types[action_type] = isPublishing(action_type);
            }
            if (publishing_types[action_type] == null)
                return null;

            return publishing_types[action_type].CreateLog(action);
        }
    }
}
