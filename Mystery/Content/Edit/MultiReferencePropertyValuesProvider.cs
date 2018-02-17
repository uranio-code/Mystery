using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mystery.Content
{
    public interface IMultiReferencePropertyValuesProvider
    {
        PropertyInfo property { get; set; }

        IEnumerable<LightContentReferece> getSuggestions(IContent item, string search_text);

        bool validate(IContent item, IEnumerable<IContent> value);

    }

    public class BaseMultiReferencePropertyValuesProvider<T> : IMultiReferencePropertyValuesProvider where T : IContent,new()
    {
        public PropertyInfo property { get; set; }

        public IEnumerable<LightContentReferece> getSuggestions(IContent item, string search_text)
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var all = cd.GetLightContentRereferece<T>();
            if (string.IsNullOrEmpty(search_text)) return all;
            return (from x in all where x.ReferenceText.Contains(search_text) select x);
        }

        public bool validate(IContent item, IEnumerable<IContent> values)
        {
            if (values == null) return true;
            foreach (IContent v in values) {
                //we do not want any values to be null in multi references
                if (v == null) return false;
                if(!typeof(T).IsAssignableFrom(v.GetType()))return false;
            }
            return true;
        }
    }

    public class MultiReferencePropertyValuesProviderAtt : MysteryPropertyAttribute
    {
        public Type implementing_type { get; set; }

        private IMultiReferencePropertyValuesProvider instance;

        public override void setup()
        {
            if (implementing_type == null) implementing_type = typeof(BaseMultiReferencePropertyValuesProvider<>).
                    MakeGenericType(this.used_in.PropertyType.GetGenericArguments().FirstOrDefault());
            if (!typeof(IMultiReferencePropertyValuesProvider).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(ISingleReferencePropertyValuesProvider).FullName);
            }
            instance = (IMultiReferencePropertyValuesProvider)this.getGlobalObject<FastActivator>().createInstance(implementing_type);
            instance.property = this.used_in;

        }

        public IMultiReferencePropertyValuesProvider getProvider()
        {
            return instance;
        }
    }

}
