using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{


    public class AutoDocGraphShape {
        public Guid guid { get; set; } = Guid.NewGuid();
        public long x { get; set; }
        public long y { get; set; }
        public ContentReference<AutoDocContentType> auto_doc_type { get; set; } = new ContentReference<AutoDocContentType>();
    }

    [ContentType]
    [ContentTypeButton()]
    [ContentTypeView()]
    [ContentView(templateUrl = "MysteryWebContent/AutoDocControls/AutoDocGraphView.html")]
    [ContentTypeTable]
    public class AutoDocGraph:BaseContent
    {
        [ContentProperty]
        [PropertyView]
        [ReferenceText]
        [SearchText]
        [PropertyColumn(template_url = "MysteryWebContent/MysteryContent/Properties/StringPropertyLinkedCell.html")]
        public string name { get; set; }

        [ContentProperty]
        [PropertyView]
        public List<AutoDocGraphShape> shapes { get; set; } = new List<AutoDocGraphShape>();

    }
}
