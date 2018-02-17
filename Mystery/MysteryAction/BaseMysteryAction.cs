using Mystery.Users;
using System;

namespace Mystery.MysteryAction
{
    public class ActionResult<ResultType> {
        public bool isSuccessfull = true;
        public bool UnAuthorized = false;
        public string message = string.Empty;
        public ResultType output;

        public ActionResult(){}
        public ActionResult(ResultType result):this() {
            output = result;
        }

        public static implicit operator ActionResult<ResultType>(ResultType initialData)
        {
            return new ActionResult<ResultType>(initialData);
        }

        public static implicit operator ResultType(ActionResult<ResultType> initialData)
        {
            if (initialData == null)
                return default(ResultType);
            return initialData.output;
        }
    }
    public class ActionResultTemplates<ResultType>
    {
        public static ActionResult<ResultType> InvalidInput { get {
                return new ActionResult<ResultType>
                {
                    isSuccessfull = false,
                    message = "invalid_input"
                };
            } }
        public static ActionResult<ResultType> UnAuthorized
        {
            get
            {
                return new ActionResult<ResultType>
                {
                    isSuccessfull = false,
                    UnAuthorized = true,
                    message = "UnAuthorized"
                };
            }
        }

        public static ActionResult<ResultType> ACK
        {
            get
            {
                return new ActionResult<ResultType>
                {
                    isSuccessfull = true,
                    message = "ACK"
                };
            }
        }
    }

    abstract public class BaseMysteryAction<InputType, ResultType>: BaseMysteryAction<ResultType>
    {
        public InputType input{get; set;}
        
    }

    /// <summary>
    /// for action which do not need a login, implement this interface
    /// </summary>
    public interface ICanRunWithOutLogin { }

    abstract public class BaseMysteryAction { }

    abstract public class BaseMysteryAction<ResultType> : BaseMysteryAction
    {
        
        public User user { get; set; }
        public User authenticated_user { get; set; }
        private bool _authorized = false;
        public bool authorized { get { return _authorized; } }

        protected abstract bool AuthorizeImplementation();

        public bool Authorize()
        {
            _authorized = AuthorizeImplementation();
            //admins shall pass
            if (!_authorized && user != null && user.account_type == UserType.admin)
                _authorized = true;
            return _authorized;
        }

        protected abstract ActionResult<ResultType> ActionImplemetation();

        public ActionResult<ResultType> Execute()
        {
            if (!_authorized) return ActionResultTemplates<ResultType>.UnAuthorized;
            return ActionImplemetation();
        }

        protected ActionResultType executeAction<ActionInputType, ActionResultType>(BaseMysteryAction<ActionInputType, ActionResultType> action, ActionInputType input)
        {
            if (!_authorized) return default(ActionResultType);
            action.input = input;
            action.user = user;
            action.authenticated_user = action.authenticated_user;
            //we do not care if authorized or not we are already in an authorized action, 
            //but the implementation might do something necessary for the action execution
            action.Authorize();
            action._authorized = true;
            return action.Execute().output;
        }
        protected ActionResultType executeAction<ActionResultType>(BaseMysteryAction<ActionResultType> action)
        {
            if (!_authorized) return default(ActionResultType);
            action.user = user;
            action.authenticated_user = action.authenticated_user;
            //we do not care if authorized or not we are already in an authorized action, 
            //but the implementation might do something necessary for the action execution
            action.Authorize();
            action._authorized = true;
            return action.Execute().output;
        }

    }


}
