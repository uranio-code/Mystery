using System;
using Mystery.Register;
using System.Collections.Generic;
using System.Linq;
using Mystery.Content;
using Mystery.Users;
using Mystery.Files;

namespace Mystery.UI
{


    public class PropertyColumn : MysteryPropertyAttribute
    {

        public string template_url { get; set; }
        
        public static IDictionary<Type, string> default_templates;


        static PropertyColumn()
        {
            default_templates = new Dictionary<Type, string>();
            default_templates[typeof(string)] = "MysteryWebContent/MysteryContent/Properties/StringPropertyCell.html";
            default_templates[typeof(double)] = "MysteryWebContent/MysteryContent/Properties/DoublePropertyCell.html";
            default_templates[typeof(long)] = "MysteryWebContent/MysteryContent/Properties/LongPropertyCell.html";
            default_templates[typeof(DateTime)] = "MysteryWebContent/MysteryContent/Properties/DatePropertyCell.html";
            default_templates[typeof(bool)] = "MysteryWebContent/MysteryContent/Properties/BooleanPropertyCell.html";
            default_templates[typeof(MysteryFile)] = "MysteryWebContent/MysteryContent/Properties/SingleFileCell.html";

        }


        public override void setup()
        {

            
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
                        "MysteryWebContent/MysteryContent/Properties/SingleReferenceCell.html" : "MysteryWebContent/MysteryContent/Properties/MultiReferenceCell.html";

                }
                else if (pt.IsEnum)
                {
                    template_url = used_in.PropertyType.IsGenericType ?
                        "MysteryWebContent/MysteryContent/Properties/SingleEnumCell.html" : "MysteryWebContent/MysteryContent/Properties/MultiEnumCell.html";
                }
                else if (pt.Equals(typeof(MysteryFile)))
                {
                    template_url = used_in.PropertyType.IsGenericType ?
                        "MysteryWebContent/MysteryContent/Properties/SingleFileCell.html" : "MysteryWebContent/MysteryContent/Properties/MultiFileCell.html";
                }
            }
                        
        }

    }
}