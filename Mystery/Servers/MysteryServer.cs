using Mystery.Content;
using Mystery.Users;
using System.Security.Cryptography;

namespace Mystery.Servers
{
    /// <summary>
    /// mystery instance can be users of one each other
    /// </summary>
    [ContentType(label = "Server", list_label = "Servers")]
    public class MysteryServer : User
    {

        [ContentProperty]
        public ContentReference<User> owner { get; set; } 


        [ContentProperty()]
        public string  rsa { get; set; } 


        private RSACryptoServiceProvider _rsa;
        public RSACryptoServiceProvider getRsa()
        {
            if (_rsa != null)
                return _rsa;

            if (string.IsNullOrEmpty(rsa))
                return null;

            RSACryptoServiceProvider result = new RSACryptoServiceProvider();
            result.PersistKeyInCsp = false;
            result.FromXmlString(rsa);
            _rsa = result;
            return result;
        }


    }
}
