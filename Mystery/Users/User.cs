
using Mystery.Content;
using System;
using Mystery.UI;
using System.Collections.Generic;
using Mystery.Register;
using System.Linq;
using Mystery.Files;

namespace Mystery.Users
{

    public enum UserType {
        normal,
        admin,
        machine,
        system
    }

    /// <summary>
    /// a user data
    /// </summary>
    /// <remarks></remarks>
    [ContentType(label = "User", list_label = "Users")]
    [Serializable()]
    [ContentTypeButton(template_url = "MysteryWebContent/MysteryContent/TypeButton.html")]
    [ContentTypeView(templateUrl = "MysteryWebContent/Users/Users.html")]
    [ContentView(templateUrl = "MysteryWebContent/Users/UserView.html")]
    [ContentTypeTable]
    public class User : BaseContent, IUser
    {

        [ContentProperty()]
        public string username { get; set; }


        [ContentProperty(label = "USER.FULLNAME")]
        [ReferenceText()]
        [PropertyView()]
        [SearchText()]
        [PropertyEdit]
        [PropertyColumn(template_url = "MysteryWebContent/MysteryContent/Properties/StringPropertyLinkedCell.html")]
        public string fullname { get; set; } 

        [ContentProperty(label = "COMMON.PASSWORD")]
        public string  password { get; set; } 


        [ContentProperty(label ="COMMON.EMAIL"), PropertyView()]
        [PropertyEdit]
        public string  email { get; set; }

        [ContentProperty]
        [PropertyView]
        public MysteryFile picture { get; set; }

        [ContentProperty()]
        [PropertyView(implementing_type = typeof(MyUserProperties))]
        public UserType account_type { get; set; }

        [ContentProperty(), PropertyView(implementing_type=typeof(MyUserProperties))]
        public ContentReference<User> working_for { get; set; } 

        [ContentProperty()]
        public string TOTPAuthenticator_key { get; set; }


        protected override void seal()
        {
            base.seal();

            string @ref = this.fullname;
            if (string.IsNullOrEmpty(@ref))
            {
                this.fullname = this.username;
            }

        }

        public IEnumerable<IContent> getFavorites() {
            var cd = this.getGlobalObject<IContentDispatcher>();
            return cd.GetAllByFilter<UserFavorite>(x => x.user.guid == this.guid);
        }

        
        public bool isFavorite(IContent content) {
            if (content == null) return false;
            var cd = this.getGlobalObject<IContentDispatcher>();
            var result =  cd.GetAllByFilter<UserFavorite>(
                x => x.user.guid == this.guid && x.content_reference.guid == content.guid)
                .FirstOrDefault() != null;
            return result;
        }
        
    }


}