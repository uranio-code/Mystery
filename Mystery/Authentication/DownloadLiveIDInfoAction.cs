using System.Linq;
using Mystery.MysteryAction;
using Mystery.Users;
using Mystery.Register;
using System.Web;
using Mystery.Content;
using Mystery.Web;
using Mystery.Configuration;
using System.Net;
using System.Text;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace Mystery.Authentication
{
    public class DownloadLiveIDInfoAction : BaseMysteryAction<string, User>, ICanRunWithOutLogin
    {


        protected override ActionResult<User> ActionImplemetation()
        {
            if (string.IsNullOrEmpty(input))
                return new ActionResult<User> { isSuccessfull = false, message = "code not given" };

            var liveIDconf = this.getGlobalObject<IConfigurationProvider>().getConfiguration<LiveIDConfiguration>();
            LiveIdAccessTokenInfo access_token_info = LiveIdAccessTokenInfo.Aquire(input, liveIDconf);

            LiveIDAccountInfo account_info = access_token_info.getAccountInfo();
            
            //live id cookies
            var cd = this.getGlobalObject<IContentDispatcher>();
            User user = cd.GetAllByFilter< User>(x=>x.email == account_info.email).FirstOrDefault();
            //first time in this instance
            if (user == null)
            {
                
                if (!liveIDconf.allow_new_users)
                    return ActionResultTemplates<User>.UnAuthorized;

                var cc = this.getGlobalObject<IGlobalContentCreator>();
                user = cc.getNewContent<User>();
            }
            user.username = account_info.preferred_username;
            user.fullname = account_info.name;
            user.email = account_info.email;
            cd.Add(user);
            
            var session = this.getGlobalObject<MysterySession>();

            session.authenticated_user = user;

            return new ActionResult<User>(user);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
