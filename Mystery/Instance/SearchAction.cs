using Mystery.Content;
using Mystery.Files;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Instance
{

    public class SearchInput
    {
        public string text { get; set; }
        public int max_result { get; set; }
    }

    /// <summary>
    /// return the current Mystery Instance View
    /// </summary>
    public class SearchAction : BaseMysteryAction<SearchInput,IEnumerable<LightContentReferece>>
    {
        protected override ActionResult<IEnumerable<LightContentReferece>> ActionImplemetation()
        {
            if (input == null)
                return ActionResultTemplates<IEnumerable<LightContentReferece>>.InvalidInput;
            if (string.IsNullOrEmpty(input.text)|| input.text.Length<3)
                return new ActionResult<IEnumerable<LightContentReferece>>();

            var cd = this.getGlobalObject<IContentDispatcher>();

            return new ActionResult<IEnumerable<LightContentReferece>>(cd.Search(input.text, input.max_result));
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
    
}
