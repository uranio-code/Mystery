using Mystery.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Users
{
    [ContentType]
    public class UserSocialLogin:BaseContent
    {
        [ContentProperty]
        public string provider_name { get; set; }

        [ContentProperty]
        public string user_unique_id { get; set; }

        [ContentProperty()]
        public ContentReference<User> user { get; set; }

    }
}
