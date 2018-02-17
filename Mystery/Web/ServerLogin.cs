using MongoDB.Bson.Serialization.Attributes;

namespace Mystery.Web
{
    public class ServerLoginToken
    {
        public string name { get; set; }
        public string secret { get; set; }
    }

    public class ServerRequestLoginOutput {
        public string server_rsa_xml { get; set; }
        public byte[] encrypted_secret { get; set; }
    }

}
