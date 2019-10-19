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

    public class SaveGraphShapeInput {
        public string grap_guid { get; set; }
        public AutoDocGraphShape shape { get; set; }
    }


    [PublishedAction(input_type: typeof(SaveGraphShapeInput), output_type: typeof(AutoDocGraph), url = nameof(SaveGraphShape))]
    public class SaveGraphShape : BaseMysteryAction<SaveGraphShapeInput, AutoDocGraph>
    {
        protected override ActionResult<AutoDocGraph> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            AutoDocGraph result = cd.GetContent<AutoDocGraph>(input.grap_guid.fromTiny());
            var shape = (from x in result.shapes where x.guid == input.shape.guid select x).FirstOrDefault();
            if (shape != null) {
                shape.x = input.shape.x;
                shape.y = input.shape.y;
            }
            cd.Add(result);
            return result;
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
