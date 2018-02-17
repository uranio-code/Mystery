using Mystery.Content;
using Mystery.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Users
{
    /// <summary>
    /// represents a user favorites, self standing content type
    /// to allow user to have many favorites without impacting user content type speed
    /// </summary>
    [ContentType()]
    public class UserFavorite: BaseContent
    {
        [ContentProperty]
        public ContentReference<User> user { get; set;}
        [ContentProperty,PropertyView()]
        public ContentReference content_reference { get; set; }
    }

    
}
