
using Mystery.Content;
using System;
using Mystery.UI;
using System.Collections.Generic;
using Mystery.Register;
using System.Linq;
using System.Collections.Concurrent;
using Mystery.Files;

namespace Mystery.Users
{
    

    /// <summary>
    /// a user data
    /// </summary>
    /// <remarks></remarks>
    [ContentType(label = "UserGroup", list_label = "UserGroups")]
    [Serializable()]
    [ContentTypeView(templateUrl = "MysteryWebContent/Users/UserGroups.html")]
    [ContentView(templateUrl = "MysteryWebContent/Users/UserGroupView.html")]
    [ContentTypeTable]
    public class UserGroup : BaseContent
    {

        [ContentProperty()]
        [PropertyEdit]
        [PropertyView]
        public string usergroupname { get; set; }

        [ContentProperty()]
        [PropertyEdit]
        [PropertyView]        
        public MultiContentReference<User> members { get; set; } 

        
    }

 
}