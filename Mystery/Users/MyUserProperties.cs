using Mystery.UI;
using Mystery.Content;

namespace Mystery.Users
{
    public class MyUserProperties: IPropertyAccess
    {
        public string template_url { get; set; }

        public bool canAccess(IContent content, User user)
        {
            //content is the actuall user
            return user != null && user.Equals(content);
        }
    }
}
