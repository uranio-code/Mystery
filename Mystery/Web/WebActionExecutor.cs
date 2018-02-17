using System;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Json;
using Mystery.UI;
using System.Collections.Generic;

namespace Mystery.Web
{

    public class WebActionResult
    {
        public bool isSuccessfull = true;
        public bool UnAuthorized = false;
        public string message = string.Empty;
        public string json_output;
    }

    public class WebActionResultTemplates {
        public static WebActionResult InvalidInput{get{
                return new WebActionResult(){
                    isSuccessfull = false,
                    message = "invalid_input",
                    UnAuthorized = false
                };}}

    }

    public class WebActionInput
    {
        public string json { get; set; }
    }

    public enum WebActionExecutorStatus {
        instanced,autheticating,gathering_input,authorizing,executing,composing_result,done,error
    }


    public class WebActionExecutor:IDisposable 
    {
        [ThreadStatic]
        static private WebActionExecutor _current;

        static public WebActionExecutor current { get { return _current; } }

        public Guid uid { get; private set; } = Guid.NewGuid();

        public List<IPublishedAction> logs { get; set; } = new List<IPublishedAction>();

        public WebActionExecutorStatus status { get; private set; } = WebActionExecutorStatus.instanced;

        public Exception exception { get; private set; }

        public event Action disposing;

        public WebActionExecutor() {
            if (_current != null) throw new Exception("you can't instance 2 " + nameof(WebActionExecutor));
            _current = this;
        }

        public WebActionResult executeAction<InputType, ResultType>(BaseMysteryAction<InputType, ResultType> action, InputType input) {

            return this.executeAction(action, () => input);
        }

        private bool autheticate<ResultType>(BaseMysteryAction<ResultType> action) {
            this.status = WebActionExecutorStatus.autheticating;
            MysterySession session = this.getGlobalObject<MysterySession>();
            if (session.authenticated_user == null && !(action is ICanRunWithOutLogin))
                return false;
            action.authenticated_user = session.authenticated_user;
            action.user = session.user;
            return true;
        }

        private WebActionResult AuthorizeExecuteComposeResult<ResultType>(BaseMysteryAction<ResultType> action)
        {
            this.status = WebActionExecutorStatus.authorizing;
            if (!action.Authorize()) {
                return new WebActionResult { isSuccessfull = false, UnAuthorized = true, message = "unauthorized" };
            };
            this.status = WebActionExecutorStatus.executing;
            ActionResult<ResultType> action_result  = action.Execute();
            var log = action.getLog();
            if(log!=null)
                logs.Add(log);
            this.status = WebActionExecutorStatus.composing_result;
            var converter = this.getGlobalObject<MysteryJsonUiConverter>();
            WebActionResult result = new WebActionResult()
            {
                isSuccessfull = action_result.isSuccessfull,
                message = action_result.message,
                UnAuthorized = action_result.UnAuthorized,
                json_output = converter.getJson(action_result.output)
            };
            this.status = WebActionExecutorStatus.done;
            return result;
        }

        public WebActionResult executeAction<InputType, ResultType>(BaseMysteryAction<InputType, ResultType> action, Func<InputType> fetchInput)
        {
            try
            {
                if (!autheticate(action))
                    return new WebActionResult { isSuccessfull = false, UnAuthorized = true, message = "unauthenticated" };

                this.status = WebActionExecutorStatus.gathering_input;
                action.input = fetchInput == null ? default(InputType) : fetchInput();
                return AuthorizeExecuteComposeResult(action);
            }
            catch (Exception ex)
            {
                this.log().Error(ex);
                this.status = WebActionExecutorStatus.error;
                this.exception = ex;
                return new WebActionResult { isSuccessfull = false, UnAuthorized = false, message = "exception" };
            }
            
        }


        public WebActionResult executeAction<ResultType>(BaseMysteryAction<ResultType> action)
        {
            try
            {
                if (!autheticate(action))
                    return new WebActionResult { isSuccessfull = false, UnAuthorized = true, message = "unauthenticated" };
                return AuthorizeExecuteComposeResult(action);
            }
            catch (Exception ex)
            {
                this.log().Error(ex);
                this.status = WebActionExecutorStatus.error;
                this.exception = ex;
                return new WebActionResult { isSuccessfull = false, UnAuthorized = false, message = "exception" };
            }
            
        }

        public void Dispose()
        {
            disposing?.Invoke();
            _current = null;
        }
    }

}
