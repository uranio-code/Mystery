using Mystery.Content;
using Mystery.Register;
using Mystery.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Mystery.History
{
    class HistoryUserJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(User);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject jo = JObject.Load(reader);
            var guid = Guid.Parse(jo.GetValue(nameof(IContent.guid)).Value<string>());
            var cd = this.getGlobalObject<IContentDispatcher>();
            var user = cd.GetContent<User>(guid);
            return user;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            User user = (User)value;

            JObject jo = new JObject();

            jo.Add(nameof(User.guid), user.guid.ToString());
            jo.Add(nameof(User.username), user.username);
            jo.Add(nameof(User.fullname), user.fullname);


            jo.WriteTo(writer);
        }
    }
}
