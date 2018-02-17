using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Files
{
    public class FilePropertyUrl : IMysteryUrl
    {
        private string _url;
        public FilePropertyUrl(IContent content, string property_name) {
            if (content == null)
                return;

            _url = nameof(FileService) + '/' + content.getContenTypeName() + '/' + content.guid.Tiny() + '/' + property_name;
        }
        public string url
        {
            get
            {
                return _url;
            }
        }
    }
}
