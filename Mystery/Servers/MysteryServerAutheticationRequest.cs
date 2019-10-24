using Mystery.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Servers
{
    /// <summary>
    /// store a authetication request from a server
    /// </summary>
    [ContentType]
    public class MysteryServerAutheticationRequest:BaseContent
    {
        //the server will sign this content guid

        [ContentProperty]
        public ContentReference<MysteryServer> server { get; set; }
    }
}
