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

    public class DownloadAutoDocMxGraphDataInput {
        public string namespace_name { get; set; }
    }
    [PublishedAction(input_type: typeof(DownloadAutoDocMxGraphDataInput), output_type: typeof(List<IContent>), url = nameof(DownloadAutoDocMxGraphData))]
    public class DownloadAutoDocMxGraphData : BaseMysteryAction<DownloadAutoDocMxGraphDataInput,List<IContent>>
    {
        protected override ActionResult<List<IContent>> ActionImplemetation()
        {

            var all_content_type_names = ContentType.getAllContentTypeNames();
            foreach (var content_type_name in all_content_type_names)
            {
                
                this.executeAction(new AutoDocGenerate(), new AutoDocGenerateInput() { content_type_name = content_type_name });
            }

            var cd = this.getGlobalObject<IContentDispatcher>();
            var result = cd.GetAll<AutoDocContentType>();
            var gone = new HashSet<AutoDocContentType>(from x in result where !all_content_type_names.Contains(x.name) select x);
            cd.RemoveContents(gone);
            result =  result.Except(gone);
            if (!string.IsNullOrWhiteSpace(input.namespace_name)) {
                result = from x in result where x.type_full_name.Contains(input.namespace_name) select x;
            }

            return new List<IContent>(result);       

        }

        protected override bool AuthorizeImplementation()
        {
            return user.account_type == Users.UserType.admin;
        }
    }
}
