using Mystery.MysteryAction;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;

namespace Mystery.UI
{

    public class PropertyEditSuggestionsActionInput {
        public ContentReference content_reference { get; set; }
        public string property_name { get; set; }

        public string search_text { get; set; }
    }


    public class SigleReferenceSuggestionsAction : BaseMysteryAction<PropertyEditSuggestionsActionInput, IEnumerable<LightContentReferece>>
    {
        protected override ActionResult<IEnumerable<LightContentReferece>> ActionImplemetation()
        {
            if (input.content_reference == null)
                return ActionResultTemplates<IEnumerable<LightContentReferece>>.InvalidInput;
            if (string.IsNullOrEmpty(input.property_name))
                return ActionResultTemplates<IEnumerable<LightContentReferece>>.InvalidInput;
            var content = this.input.content_reference.getContent();
            if (content == null)
                return ActionResultTemplates<IEnumerable<LightContentReferece>>.InvalidInput;

            var property = (from x in content.GetType().getMysteryPropertyAttributes<ContentProperty>()
                            where x.name == input.property_name
                            select x).FirstOrDefault();

            if (property == null)
                return ActionResultTemplates<IEnumerable<LightContentReferece>>.InvalidInput;

            var attr = property.used_in.getMysteryAttribute<SingleReferencePropertyValuesProviderAtt>();

            if (attr == null) 
                return ActionResultTemplates<IEnumerable<LightContentReferece>>.InvalidInput;

            var provider = attr.getProvider();

            return new ActionResult<IEnumerable<LightContentReferece>>(
                provider.getSuggestions(content, input.search_text));
            
        }

        protected override bool AuthorizeImplementation()
        {
            //you can if you can edit it
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

            var pe = property.used_in.getMysteryAttribute<PropertyEdit>();

            return pe!=null?pe.getPropertyEdit().canEdit(content, user):false;

        }
    }
}
