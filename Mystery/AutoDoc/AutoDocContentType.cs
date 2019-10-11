using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AutoDoc
{

    public class AutoDocContentTypeReference {
        public string name { get; set; }
        public bool multi { get; set; }
        public string target_type { get; set; }
    }

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
        public string type_full_name { get; set; }

        [ContentProperty]
        [PropertyView]
        public List<string> properties_names { get; set; }

        [ContentProperty]
        [PropertyView]
        public Dictionary<string, AutoDocContentTypeReference> references { get; set; } = new Dictionary<string, AutoDocContentTypeReference>();

        
    }
}
