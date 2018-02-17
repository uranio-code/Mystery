using Mystery.Content;
using System;
using Mystery.UI;
using Mystery.Users;

namespace Mystery.Comment
{

    public enum CommentNature
    {
        normal,
        warring,
        critical,
    }

    
    [ContentType()]
    public class Comment: BaseContent
    {

        [ContentProperty()]
        public ContentReference<User> author { get; set; } 

        [ContentProperty()]
        public string comment { get; set; } 
        
        [ContentProperty()]
        public CommentNature type { get; set; } 


    }

}