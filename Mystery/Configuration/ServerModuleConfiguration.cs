using Mystery.Content;
using Mystery.Web;

namespace Mystery.Configuration
{
    [ContentType()]
    public class ServerModuleConfiguration : BaseContent
    {

        [ContentProperty()]
        public ContentReference<MysteryServer> server { get; set; } = new ContentReference<MysteryServer>();

        [ContentProperty()]
        public string server_name { get {
                if (server.value == null)
                    return string.Empty;
                return server.value.username;
            } } 

        [ContentProperty()]
        public string configuration_name { get; set; } 

        [ContentProperty()]
        public string  version { get; set; } 

        [ContentProperty()]
        public string  evn_name { get; set; } 

        [ContentProperty()]
        public string  json_data { get; set; } 

    }
}
