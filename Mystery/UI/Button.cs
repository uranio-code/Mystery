using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UI
{
    public class Button
    {
        public string label { get; set; }
        public string style { get; set; } = "default";
        public string font_awesome_icon { get; set; } 
        public string font_awesome_icon_2 { get; set; }
        public string tootip { get; set; }
        public bool enabled { get; set; } = true;
    }
}
