using Mystery.MysteryAction;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;
using Mystery.Json;

namespace Mystery.UI
{

    public class PropertyEditActionData : WhoWhatWhen {
        public string property_name { get; set; }
    }

    [PublishedAction(input_type:typeof(MonoPropertyContent),url = nameof(PropertyEditAction))]
    public class PropertyEditAction : BaseMysteryAction<MonoPropertyContent, ContentActionOutput>,IPublishedAction<PropertyEditActionData>
    {
        public PropertyEditActionData history_message_data { get; private set; }

        public string history_message_template_url
        {
            get
            {
                return "MysteryWebContent/History/PropertyEditHistory.html";
            }
        }

        public List<string> history_tags { get; private set; } = new List<string>();

        public bool has_history { get; private set; }

        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            if (input == null)
                return ActionResultTemplates< ContentActionOutput>.InvalidInput;
            if (input.content_reference == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;
            if (string.IsNullOrEmpty(input.property_name))
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;
            var content = this.input.content_reference.getContent();
            if (content == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var property = (from x in content.GetType().getMysteryPropertyAttributes<ContentProperty>()
                            where x.name == input.property_name
                            select x).FirstOrDefault();

            if (property == null)
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var converter = this.getGlobalObject<MysteryJsonConverter>();

            //we make a json comparision to see if we actually edit it
            var before = converter.getJson(content);
            property.save(content, input.property_value);
            var after = converter.getJson(content);

            has_history = before != after;


            var result = new ContentActionOutput();
            if (has_history) {
                result.changed_contents.Add(content);

                history_message_data = new PropertyEditActionData()
                {
                    property_name = input.property_name,
                    who = user.guid,
                    what = new ContentReference(content),
                };
                history_tags.Add(nameof(PropertyEditAction));
                history_tags.Add(content.getContenTypeName());
            }

            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            if (input == null)
                return false;
            if (input.content_reference == null)
                return false;
            if (string.IsNullOrEmpty(input.property_name))
                return false;
            var content = this.input.content_reference.getContent();
            if (content == null)
                return false;

            var property = (from x in content.GetType().getMysteryPropertyAttributes<ContentProperty>()
                                where x.name == input.property_name
                                select x).FirstOrDefault();

            if (property == null)
                return false;
            var pe_attr = property.used_in.getMysteryAttribute<PropertyEdit>();
            if (pe_attr == null)
                return false;
            var pe = pe_attr.getPropertyEdit();
            if (pe == null)
                return false;
            return pe.canEdit(content, user);

        }
    }
}
