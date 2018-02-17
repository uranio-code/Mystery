using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Content
{
    public interface ISingleReferencePropertyValuesProvider
    {
        PropertyInfo property { get; set; }

        IEnumerable<LightContentReferece> getSuggestions(IContent item, string search_text);

        bool validate(IContent item, IContent value);

    }

    public class BaseSingleReferencePropertyValuesProvider<T> : ISingleReferencePropertyValuesProvider where T:IContent, new()
    {
        public PropertyInfo property { get; set; }
        
        public IEnumerable<LightContentReferece> getSuggestions(IContent item, string search_text)
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var all = cd.GetLightContentRereferece<T>();
            if (string.IsNullOrEmpty(search_text)) return all;
            return (from x in all where x.ReferenceText.Contains(search_text) select x);
        }

        public bool validate(IContent item, IContent value)
        {
            if (value == null) return true;
            return typeof(T).IsAssignableFrom(value.GetType());
        }
    }

    public class SingleReferencePropertyValuesProviderAtt : MysteryPropertyAttribute
    {
        public Type implementing_type { get; set; }

        private ISingleReferencePropertyValuesProvider instance;

        public override void setup()
        {
            if (implementing_type == null) implementing_type = typeof(BaseSingleReferencePropertyValuesProvider<>).
                    MakeGenericType(this.used_in.PropertyType.GetGenericArguments().FirstOrDefault());
            if (!typeof(ISingleReferencePropertyValuesProvider).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(ISingleReferencePropertyValuesProvider).FullName);
            }
            instance = (ISingleReferencePropertyValuesProvider)this.getGlobalObject<FastActivator>().createInstance(implementing_type);
            instance.property = this.used_in;

        }

        public ISingleReferencePropertyValuesProvider getProvider() {
            return instance;
        } 
    }

}
