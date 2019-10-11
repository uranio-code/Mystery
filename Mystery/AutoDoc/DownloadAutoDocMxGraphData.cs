using com.mxgraph;
using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{


    [PublishedAction(input_type: null, output_type: typeof(List<IContent>), url = nameof(DownloadAutoDocMxGraphData))]
    public class DownloadAutoDocMxGraphData : BaseMysteryAction<List<IContent>>
    {
        protected override ActionResult<List<IContent>> ActionImplemetation()
        {

            var all_content_type_names = ContentType.getAllContentTypeNames();
            foreach (var content_type_name in all_content_type_names)
            {
                this.executeAction(new AutoDocGenerate(), new AutoDocGenerateInput() { content_type_name = content_type_name });
            }

            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = new List<IContent>(cd.GetAll<AutoDocContentType>());
            return result;

        }

        protected override bool AuthorizeImplementation()
        {
            return user.account_type == Users.UserType.admin;
        }
    }
}
