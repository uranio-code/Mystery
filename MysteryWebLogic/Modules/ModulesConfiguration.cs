using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryWebLogic.Modules
{
    public class MysterySiteConfiguration
    {

        public string name { get; set; } = "MysteryWebUi";

        public string path { get; set; } 

        public List<string> projects { get; set; } 


        public List<ModuleConfiguration> modules { get; set; } = new List<ModuleConfiguration>();

        public IEnumerable<ModuleConfiguration> active_modules { get {
                return from x in this.modules
                       where x.active
                       select x;
            } }
    }

    public class ModuleConfiguration {
        public string name { get; set; }

        public string dll_name { get; set; }

        public string source_project_path { get; set; }

        public string web_content_virtual_directory_name { get; set; }

        public string web_content_path { get; set; }

        public bool active { get; set; }
    }
    

}
