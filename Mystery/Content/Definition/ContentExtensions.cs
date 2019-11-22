using Mystery.Json;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Users;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.Content
{
    public static class ContentExtensions
    {

        public static bool sameContents(this IEnumerable<IContent> first, IEnumerable<IContent> second)
        {
            if (first == null && second == null)
                return true;
            if (first == null )
                return false;
            if (second == null)
                return false;
            HashSet<IContent> base_hash = new HashSet<IContent>(first);
            HashSet<IContent> other_hash = new HashSet<IContent>(second);
            if (base_hash.Count != other_hash.Count)
                return false;
            return base_hash.Intersect(other_hash).Count() == base_hash.Count();
        }
        public static bool sameContents(this IEnumerable<Guid> first, IEnumerable<Guid> second)
        {
            if (first == null && second == null)
                return true;
            if (first == null)
                return false;
            if (second == null)
                return false;
            HashSet<Guid> base_hash = new HashSet<Guid>(first);
            HashSet<Guid> other_hash = new HashSet<Guid>(second);
            if (base_hash.Count != other_hash.Count)
                return false;
            return base_hash.Intersect(other_hash).Count() == base_hash.Count();
        }



        /// <summary>
        /// take all the property with the same name and type
        /// </summary>
        /// <param name="source_content">content target</param>
        /// <param name="target_content">content source</param>
        /// <remarks></remarks>
        public static void copyTo(this IContent source_content, IContent target_content)
        {
            if (source_content == null || target_content == null)
                return;

            Type source_type = source_content.GetType();
            Type target_type = target_content.GetType();
            

            IEnumerable<ContentProperty> all = source_type.getMysteryPropertyAttributes<ContentProperty>();
            HashSet<ContentProperty> source_properties = new HashSet<ContentProperty>((from x in all where x.retrive != null select x));
            
            all = target_type.getMysteryPropertyAttributes<ContentProperty>();
            HashSet<ContentProperty> target_properties = new HashSet<ContentProperty>((from x in all where x.retrive != null select x));
            
            Dictionary<string, MysteryPropertyAttribute> source_map = new Dictionary<string, MysteryPropertyAttribute>();
            foreach (ContentProperty sp in source_properties)
            {
                source_map[sp.name] = sp;
            }
            
            Dictionary<string, MysteryPropertyAttribute> target_map = new Dictionary<string, MysteryPropertyAttribute>();
            foreach (ContentProperty tp in target_properties)
            {
                target_map[tp.name] = tp;
            }
            
            foreach (string pn in target_map.Keys)
            {
                try
                {
                    if (!source_map.ContainsKey(pn))
                        continue;
                    MysteryPropertyAttribute source = source_map[pn];
                    MysteryPropertyAttribute target = target_map[pn];
                    if ((!object.ReferenceEquals(source.used_in.PropertyType, target.used_in.PropertyType)))
                        continue;
                    target.save(target_content, source.retrive(source_content));
                }
                catch 
                {
                    //well if some failed is not the end of the world, is it?
                }
            }

        }
        public static bool isSameAs<T>(this IContentReference source, ContentReference<T> target) where T:IContent,new()
        {
            if (source == null && target == null)
                return true;
            //one of them is not null, so what to use Equals class methods
            if (source != null)
                return source.Equals(target);
            return target.Equals(source);
        }
        public static bool isSameAs(this IContentReference source, IContentReference target) {
            if (source == null && target == null)
                return true;
            //one of them is not null, so what to use Equals class methods
            if (source != null)
                return source.Equals(target);
            return target.Equals(source);
        }
        public static bool isSameAs<T>(this IContentReference source, T target) where T:IContent,new()
        {
            return isSameAs(source,(IContentReference)new ContentReference<T>(target));
        }
        public static bool isReferedBy<T>(this T source, IContentReference target) where T:IContent,new()
        {
            return target.isSameAs(source);
        }

        public static bool isJsonEquivalent(this IContent source_content, IContent target_content) {
            if (source_content == null && target_content == null)
                return true;
            if (source_content == null || target_content == null)
                return false;
            if (ReferenceEquals(source_content, target_content))
                return true;
            var converter = source_content.getGlobalObject<IMysteryJsonConverter>();
            var result = converter.getJson(source_content) == converter.getJson(target_content);
            return result;
        }

        /// <summary>
        /// compute which property are different
        /// </summary>
        /// <param name="source_content"></param>
        /// <param name="target_content"></param>
        /// <returns></returns>
        public static IEnumerable<string> getDifferenceWith(this IContent source_content, IContent target_content)
        {
            if (source_content.isJsonEquivalent(target_content))
                return new List<string>();
            var converter = source_content.getGlobalObject<IMysteryJsonConverter>();
            var source_json = converter.getJson(source_content);
            var target_json = converter.getJson(target_content);

            // convert JSON to object
            JObject xptJson = JObject.Parse(source_json);
            JObject actualJson = JObject.Parse(target_json);

            // read properties
            var xptProps = xptJson.Properties().ToList();
            var actProps = actualJson.Properties().ToList();

            // find differing properties
            var result = (from existingProp in actProps
                            from modifiedProp in xptProps
                            where modifiedProp.Path.Equals(existingProp.Path)
                            where !modifiedProp.Value.ToString().Equals(existingProp.Value.ToString())
                            select existingProp.Path).ToList();
                            
            return result;
        }

        /// <summary>
        /// compare all the properties between target content and source content.
        /// </summary>
        /// <param name="source_content">content target</param>
        /// <param name="target_content">content source</param>
        /// <remarks></remarks>
        public static bool samePropertiesValue(this IContent source_content, IContent target_content)
        {
            if (source_content == null && target_content == null)
                return true;
            if (source_content == null || target_content == null)
                return false;

            Type source_type = source_content.GetType();
            Type target_type = target_content.GetType();
            

            IEnumerable<ContentProperty> all = source_type.getMysteryPropertyAttributes<ContentProperty>();
            HashSet<ContentProperty> source_properties = new HashSet<ContentProperty>((from x in all where x.retrive != null select x));
            
            all = target_type.getMysteryPropertyAttributes<ContentProperty>();
            HashSet<ContentProperty> target_properties = new HashSet<ContentProperty>((from x in all where x.retrive != null select x));
            
            Dictionary<string, MysteryPropertyAttribute> source_map = new Dictionary<string, MysteryPropertyAttribute>();
            foreach (ContentProperty sp in source_properties)
            {
                source_map[sp.name] = sp;
            }
           
            
            foreach (ContentProperty tp in target_properties)
            {
                try
                {
                    if (!source_map.ContainsKey(tp.name))
                        continue;
                    MysteryPropertyAttribute source = source_map[tp.name];
                    if ((!object.ReferenceEquals(source.used_in.PropertyType, tp.used_in.PropertyType)))
                        continue;
                    object source_value = source.retrive(source_content);
                    object target_value = tp.retrive(target_content);
                    if (source_value == null && target_value == null)
                        continue;
                    //hating datetime a little more every day, when we store them in a db or js or anything on their way
                    //back they might differe a little, equals compare the ticks so it is would have to be quite close.
                    //in case of date we assume the same value if they differ for less than 1ms.
                    if (source_value is DateTime && target_value is DateTime && source_value != null && target_value != null)
                        if (((DateTime)source_value - (DateTime)target_value).TotalMilliseconds < 1)
                            continue;
                    //one of them is not null, so what to use Equals class methods
                    if (source_value != null && !source_value.Equals(target_value))
                        return false;
                    if (target_value != null && !target_value.Equals(source_value))
                        return false;
                }
                catch
                {
                    return false;
                }
            }
            
            return true;
        }

        public static string getContenTypeName(this IContent content) {
            if (content == null) return null;
            return content.GetType().getMysteryAttribute<ContentType>().name;
        }

        public static bool canAccess(this IContent content, User user)
        {
            if (content == null) return true;
            return content.GetType().getMysteryAttribute<ContentAccessAttribute>().canAccess(content, user);
        }

    }
}
