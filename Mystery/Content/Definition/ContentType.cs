using Mystery.Register;
using System;
using System.Collections.Generic;
namespace Mystery.Content
{

    /// <summary>
    /// attribute for class representing a content
    /// </summary>
    /// <remarks></remarks>
    public class ContentType : MysteryClassAttribute
    {


        /// <summary>
        /// define it if you would like than for example it shall be read  and store with a different names
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>by default the class name (without namespace) will be used
        public string name { get; set; }

        /// <summary>
        /// how it should be called in the UI
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>by default the class name (without namespace)</remarks>
        public string label { get; set; }

        /// <summary>
        /// how it should be called in the UI when more then one
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>by default the class name (without namespace)</remarks>
        public string list_label { get; set; }

        private static IDictionary<string, Type> map = new Dictionary<string, Type>();
        public static Type getType(string content_type_name) {
            if (!map.ContainsKey(content_type_name))
                return null;
            return map[content_type_name];
        }

        public static IEnumerable<Type> getAllContentTypes() {
            return map.Values;
        }
        public static IEnumerable<string> getAllContentTypeNames() {
            return map.Keys;
        }

        public override void setUp()
        {
            if (string.IsNullOrEmpty(name))
                name = used_in.Name;
            if (string.IsNullOrEmpty(label))
                label = name;
            if (string.IsNullOrEmpty(list_label))
                list_label = label;
            map[name] = used_in;
        }
    } 
}