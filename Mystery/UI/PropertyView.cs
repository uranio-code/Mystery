using System;
using Mystery.Register;
using System.Collections.Generic;
using System.Linq;
using Mystery.Content;
using Mystery.Users;
using Mystery.Files;

namespace Mystery.UI
{

    public interface IPropertyAccess
    {
        string template_url { get; set; }

        bool canAccess(IContent content, User user);
    }

    public class AllCanAccess : IPropertyAccess
    {
        public string template_url { get; set; }

        public bool canAccess(IContent content, User user)
        {
            return true;
        }
    }

    public class PropertyView : MysteryPropertyAttribute
    {

        public string template_url { get; set; }

        public Type implementing_type { get; set; }

        private IPropertyAccess instance;

        public static IDictionary<Type, string> default_templates;

        public string help_html { get; set; }

        static PropertyView()
        {
            default_templates = new Dictionary<Type, string>();
            default_templates[typeof(string)] = "MysteryWebContent/MysteryContent/Properties/StringProperty.html";
            default_templates[typeof(double)] = "MysteryWebContent/MysteryContent/Properties/DoubleProperty.html";
            default_templates[typeof(long)] = "MysteryWebContent/MysteryContent/Properties/LongProperty.html";
            default_templates[typeof(DateTime)] = "MysteryWebContent/MysteryContent/Properties/DateProperty.html";
            default_templates[typeof(bool)] = "MysteryWebContent/MysteryContent/Properties/BooleanProperty.html";
            default_templates[typeof(MysteryFile)] = "MysteryWebContent/MysteryContent/Properties/SingleFile.html";

        }


        public override void setup()
        {

            if (implementing_type == null) implementing_type = typeof(AllCanAccess);
            if (!typeof(IPropertyAccess).IsAssignableFrom(implementing_type))
            {
                throw new Exception(implementing_type.FullName + " must implement " + typeof(IPropertyAccess).FullName);
            }
            instance = (IPropertyAccess)Activator.CreateInstance(implementing_type);

            if (string.IsNullOrEmpty(template_url))
            {
                Type pt = used_in.PropertyType.IsGenericType ?
                    used_in.PropertyType.GetGenericArguments().FirstOrDefault() :
                    used_in.PropertyType;

                if (default_templates.ContainsKey(pt))
                    template_url = default_templates[pt];
                else if (typeof(IContent).IsAssignableFrom(pt))
                {

                    template_url = ReferenceEquals(used_in.PropertyType.GetGenericTypeDefinition(), typeof(ContentReference<>)) ?
                        "MysteryWebContent/MysteryContent/Properties/SingleReference.html" : "MysteryWebContent/MysteryContent/Properties/MultiReference.html";

                }
                else if (pt.IsEnum)
                {
                    template_url = used_in.PropertyType.IsGenericType ?
                        "MysteryWebContent/MysteryContent/Properties/SingleEnum.html" : "MysteryWebContent/MysteryContent/Properties/MultiEnum.html";
                }
                else if (pt.Equals(typeof(MysteryFile)))
                {
                    template_url = used_in.PropertyType.IsGenericType ?
                        "MysteryWebContent/MysteryContent/Properties/SingleFile.html" : "MysteryWebContent/MysteryContent/Properties/MultiFile.html";
                }
            }
            if (string.IsNullOrEmpty(instance.template_url))
            {
                instance.template_url = template_url;
            }
                        
        }

        public IPropertyAccess getPropertyAccess() {
            return instance;
        }

    }
}