using Mystery.MysteryAction;
using Mystery.Users;
using Mystery.Register;
using Mystery.Web;
using System;

namespace Mystery.Authentication
{
    public class LoginInput
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class LoginAction : BaseMysteryAction<LoginInput, User>, ICanRunWithOutLogin 
    {
        
        protected override ActionResult<User> ActionImplemetation()
        {

            IAuthetication au = this.getGlobalObject<IAuthetication>();
            User result = au.autheticate(input.email, input.password);
            this.getGlobalObject<MysterySession>().authenticated_user = result;
            return new ActionResult<User>(result) { isSuccessfull = result != null };
        }

        protected override bool AuthorizeImplementation()
        {
            //anybody can login
            return true;
        }
    }

    public class LogoutAction : BaseMysteryAction<string>
    {
        protected override ActionResult<string> ActionImplemetation()
        {

            var session = this.getGlobalObject<MysterySession>();
            MysterySession.releaseSession(session.id);
            return new ActionResult<string>(nameof(LogoutAction));
        }

        protected override bool AuthorizeImplementation()
        {
            //anybody can logout
            return true;
        }
    }

    public class GetMe : BaseMysteryAction<User>, ICanRunWithOutLogin
    {
        protected override ActionResult<User> ActionImplemetation()
        {
            var session = this.getGlobalObject<MysterySession>();
            return session.user;
        }

        protected override bool AuthorizeImplementation()
        {
            //anybody can get it own data
            return true;
        }
    }

    //tool action to have the client login
    public class LoggedEcho : BaseMysteryAction<string>
    {
        protected override ActionResult<string> ActionImplemetation()
        {
            return new ActionResult<string>(nameof(LoggedEcho));
        }

        protected override bool AuthorizeImplementation()
        {
            //anybody logged can echo
            return true;
        }
    }
}