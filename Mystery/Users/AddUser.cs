using Mystery.MysteryAction;
using Mystery.UI;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;

namespace Mystery.Users
{
    public class AddUserInput {
        public string fullname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
    [PublishedAction(input_type: typeof(AddUserInput), url = nameof(AddUser))]
    public class AddUser : BaseMysteryAction<AddUserInput, ContentActionOutput>
    {
        protected override ActionResult<ContentActionOutput> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            if (string.IsNullOrWhiteSpace(input.email))
                return ActionResultTemplates<ContentActionOutput>.InvalidInput;

            var existing = cd.GetAllByFilter<User>(x => x.email == input.email).FirstOrDefault();
            if (existing != null)
                return new ActionResult<ContentActionOutput>()
                {
                    isSuccessfull = false,
                    message = "USER.ALREADY_EXISTING",
                    output = new ContentActionOutput() { contents = {existing } }
                };
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var user = cc.getNewContent<User>();
            user.fullname = input.fullname;
            user.email = input.email;
            user.password = this.getGlobalObject<IAuthetication>().Hashstring(input.password);
            cd.Add(user);

            return new ActionResult<ContentActionOutput>(new ContentActionOutput()
            {
                new_contents = { user },
                next_url = new ContentUrl(user)
            });
            
        }

        protected override bool AuthorizeImplementation()
        {
            return user.account_type == UserType.admin;
        }
    }
}
