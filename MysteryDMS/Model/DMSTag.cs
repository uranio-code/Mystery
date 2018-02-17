using Mystery.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryDMS.Model
{
    [ContentType(label = "Tag", list_label = "Tag")]
    public class DMSTag : BaseContent
    {
        [ContentProperty(label = "DMS.TAGS.TITLE"), ReferenceText()]
        public string  title { get; set; } 
    }
}
