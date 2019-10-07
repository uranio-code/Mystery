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

        private static Dictionary<string, Guid> auto_doc_map = new Dictionary<string, Guid>();


        private AutoDocContentType getAutoDoc(string content_type_name, ContentActionOutput result) {
            var cd = this.getGlobalObject<IContentDispatcher>();
            if (auto_doc_map.ContainsKey(content_type_name))
                return cd.GetContent<AutoDocContentType>(auto_doc_map[content_type_name]);
            var auto_doc = cd.GetAllByFilter<AutoDocContentType>((x) => x.name == content_type_name).FirstOrDefault();
            if (auto_doc == null)
            {
                var cc = this.getGlobalObject<IGlobalContentCreator>();
                auto_doc = cc.getAndAddNewContent<AutoDocContentType>();
                auto_doc.name = content_type_name;
                result.new_contents.Add(auto_doc);
            }
            auto_doc_map[content_type_name] = auto_doc.guid;
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
            // this action is recursive, if auto_doc_map contains already this type it mean
            // it is already under contruction in a parent call
            if (auto_doc_map.ContainsKey(content_type_name)) {
                result.main = cd.GetContent<AutoDocContentType>(auto_doc_map[content_type_name]);
                return result;
            }

            var auto_doc = getAutoDoc(content_type_name,result);


            result.main = auto_doc;
            result.next_url = new ContentUrl(auto_doc);

            auto_doc.properties_names = new List<string>();
            var single_references = new List<AutoDocContentType>();
            var multi_references = new List<AutoDocContentType>();
            //properties
            foreach (var content_property in type.getMysteryPropertyAttributes<ContentProperty>()) {
                auto_doc.properties_names.Add(content_property.name);
                //reference?
                if (typeof(IReferenceProperty).IsAssignableFrom(content_property.used_in.PropertyType)) {
                    var is_sigle_reference = ReferenceEquals(content_property.used_in.PropertyType.GetGenericTypeDefinition(), typeof(ContentReference<>));
                    var reffered_content_type_name = content_property.used_in.PropertyType.
                        GenericTypeArguments.FirstOrDefault().getMysteryAttribute<ContentType>().name;
                    //going recursiove
                    var recursion_result = executeAction(
                            new AutoDocGenerate(),
                            new AutoDocGenerateInput() { content_type_name = reffered_content_type_name });
                    result.changed_contents.AddRange(recursion_result.changed_contents);
                    result.new_contents.AddRange(recursion_result.new_contents);
                    result.deleted_contents.AddRange(recursion_result.deleted_contents);
                    var reference_auto_doc = (AutoDocContentType)recursion_result.main;
                    if (is_sigle_reference)
                    {
                        single_references.Add(reference_auto_doc);
                    }
                    else {
                        multi_references.Add(reference_auto_doc);
                    }
                }
            }

            auto_doc.single_references = new MultiContentReference<AutoDocContentType>(single_references);
            auto_doc.multi_references = new MultiContentReference<AutoDocContentType>(multi_references);

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
