using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Applications
{
    [ContentType]
    public class MysteryApplication:BaseContent
    {
        [ContentProperty, PropertyView]
        public string name { get; set; }

        [ContentProperty, PropertyView]
        public string label { get; set; }

        [ContentProperty, PropertyView]
        public string short_label { get; set; }

        [ContentProperty, PropertyView]
        public string start_page { get; set; }


        [ContentProperty]
        public string dll_name { get; set; }

        [ContentProperty, PropertyView]
        public bool active { get; set; }

        [ContentProperty, PropertyView]
        public bool installed { get; set; }
    }


}
