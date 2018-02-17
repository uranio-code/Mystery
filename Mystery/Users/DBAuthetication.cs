
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Mystery.Register;
using Mystery.Content;
namespace Mystery.Users
{

    /// <summary>
    /// check if the given user is in the database
    /// </summary>
    /// <remarks></remarks>
    [GlobalAvalibleObjectImplementation(implementation_of = typeof(IAuthetication), overrides_exsisting = false, singleton = true)]
    public class UserContentAutheticator : IAuthetication
    {


        private static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

        public string Hashstring(string input)
        {
            byte[] hash = md5.ComputeHash(input.getBytes());
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public User autheticate(string email, string password)
        {
            IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();
            string pass = Hashstring(password);
            return cd.GetAllByFilter<User>(x=>x.email == email && x.password == pass).FirstOrDefault();
        }
    }

}