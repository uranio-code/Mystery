using Mystery.MysteryAction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Content;
using Mystery.TOTPAuthentication;
using Mystery.Instance;

namespace Mystery.Users
{

    class TOTPAuthenticationKeyActionOuput {
        public string instance_name { get; set; }
        public string upn { get; set; }
        public string key { get; set; }
    }

    class TOTPAuthenticationKeyAction : BaseMysteryAction<TOTPAuthenticationKeyActionOuput>
    {
        protected override ActionResult<TOTPAuthenticationKeyActionOuput> ActionImplemetation()
        {
            if (string.IsNullOrWhiteSpace(user.TOTPAuthenticator_key)) {
                var cd = this.getGlobalObject<IContentDispatcher>();
                var autheticator = this.getGlobalObject<TOTPAuthenticator>();
                user.TOTPAuthenticator_key = autheticator.GenerateSecretKey();
                cd.Add(user);
            }
            var instance = this.getGlobalObject<MysteryInstance>();
            return new TOTPAuthenticationKeyActionOuput { upn = user.email, key = user.TOTPAuthenticator_key, instance_name = instance.name };
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }

    public class TOTPAuthenticationCodeInput
    {
        public string code { get; set; }
    }

    class TOTPAuthenticationCodeAction : BaseMysteryAction<string,bool>
    {
        protected override ActionResult<bool> ActionImplemetation()
        {
            var autheticator = this.getGlobalObject<TOTPAuthenticator>();
            return autheticator.CheckCode(user.email, user.TOTPAuthenticator_key, input);
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}
