using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{

    public class AutoDocGenerateInput {
        public string content_type_name { get; set; }
    }

    [PublishedAction(input_type: typeof(AutoDocGenerateInput), output_type: typeof(ContentActionOutput), url = nameof(AutoDocGenerate))]
    public class AutoDocGenerate : BaseMysteryAction<AutoDocGenerateInput, ContentActionOutput>
    {

        


        private AutoDocContentType getAutoDoc(string content_type_name, ContentActionOutput result) {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var auto_doc = cd.GetAllByFilter<AutoDocContentType>((x) => x.name == content_type_name).FirstOrDefault();
            if (auto_doc == null)
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                auto_doc = cc.getAndAddNewContent<AutoDocContentType>();
                auto_doc.name = content_type_name;
                result.new_contents.Add(auto_doc);
            }
            return auto_doc;
        }

        

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var content_type_name = input.content_type_name;
            if (String.IsNullOrWhiteSpace(content_type_name))
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;
            var type = ContentType.getType(content_type_name);
            if(type == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = new ContentActionOutput();
            
            var auto_doc = getAutoDoc(content_type_name,result);
            auto_doc.type_full_name = type.FullName;


            result.main = auto_doc;
            result.next_url = new ContentUrl(auto_doc);

            auto_doc.properties_names = new List<string>();
            auto_doc.references = new Dictionary<string, AutoDocContentTypeReference>();
            
            //properties
            foreach (var content_property in type.getMysteryPropertyAttributes<ContentProperty>()) {
                auto_doc.properties_names.Add(content_property.name);
                //reference?
                if (typeof(IReferenceProperty).IsAssignableFrom(content_property.used_in.PropertyType)) {
                    var entry = new AutoDocContentTypeReference();

                    entry.multi = ReferenceEquals(content_property.used_in.PropertyType.GetGenericTypeDefinition(), typeof(MultiContentReference<>));
                    entry.target_type = content_property.used_in.PropertyType.
                        GenericTypeArguments.FirstOrDefault().getMysteryAttribute<ContentType>().name;
                    entry.name = content_property.name;
                    auto_doc.references[entry.name] = entry;
                }
            }

            result.changed_contents.Add(auto_doc);
            cd.Add(auto_doc);

            return result;

        }

        protected override bool AuthorizeImplementation()
        {
            return user?.account_type == Users.UserType.admin;
        }
    }
}
