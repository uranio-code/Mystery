using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{



    [ContentType]
    [ContentTypeButton()]
    [ContentTypeView()]
    [ContentView()]
    [ContentTypeTable]
    public class AutoDocContentType:BaseContent
    {
        [ContentProperty]
        [PropertyView]
        public string name { get; set; }

        [ContentProperty]
        [PropertyView]
        public List<string> properties_names { get; set; }

        [ContentProperty]
        [PropertyView]
        public MultiContentReference<AutoDocContentType> single_references { get; set; }

        [ContentProperty]
        [PropertyView]
        public MultiContentReference<AutoDocContentType> multi_references { get; set; }

    }
}
