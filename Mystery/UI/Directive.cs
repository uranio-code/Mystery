using Mystery.Content;
using Mystery.Json;
using Mystery.Register;
using System.Collections.Generic;
using System.Linq;

namespace Mystery.UI
{

    [ContentType()]
    public class MysteryDirective : BaseContent
    {
        [ContentProperty]
        public string  name { get; set; } 

        [ContentProperty]
        public string  templateUrl { get; set; } 

        [ContentProperty]
        public string  template { get; set; } 

        /// <summary>
        /// comma separated list of scope variable
        /// </summary>
        [ContentProperty]
        public string scopes { get; set; } 

        public string getJs() {
            var converter = this.getGlobalObject<IMysteryJsonConverter>();
            string result = "app.directive(" + converter.getJson(name) + ", function() {return {restrict: 'E',";

            if (!string.IsNullOrEmpty(this.scopes)) {
                var names = new List<string>(
                    from x in this.scopes.Split(',')
                    where !string.IsNullOrEmpty(x.Trim())
                    select x.Trim()
                    );

                var jo = "{" + string.Join(",",
                    from x in names
                    select x + ": '=" + x + "'"
                    ) + "}";
                result += "scope:" + jo +",";
            }
            
            if (!string.IsNullOrEmpty(template))
                result += "template:" + converter.getJson(template);
            else
                result += "templateUrl:" + converter.getJson(templateUrl);
            result += "};});";
            return result;
        }
    }
}
