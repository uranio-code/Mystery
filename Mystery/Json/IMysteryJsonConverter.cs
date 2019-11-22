using Mystery.Register;
using System;

namespace Mystery.Json
{
    [GlobalAvalibleObject()]
    public interface IMysteryJsonConverter
    {
        string getJson(object item);
        object readJson(string json, Type objectType);
        ObjectType readJson<ObjectType>(string json);
    }
}