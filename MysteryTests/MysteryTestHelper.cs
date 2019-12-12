using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mystery.MysteryAction;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysteryTests
{
    /// <summary>
    /// covience class to help testing
    /// </summary>
    public class MysteryTestHelper
    {

        private class MyAction<InputType, OutputType, ActionType> : 
            BaseMysteryAction<InputType, OutputType>, ICanRunWithOutLogin 
            where ActionType : BaseMysteryAction<InputType, OutputType>, new()
        {
            public OutputType action_result { get; set; }
            protected override ActionResult<OutputType> ActionImplemetation()
            {
                var inner_action = new ActionType();
                action_result = executeAction(inner_action, input);
                return action_result;
            }

            protected override bool AuthorizeImplementation()
            {
                return true;
            }
        }
        private class MyAction:
            BaseMysteryAction<bool>, ICanRunWithOutLogin
        {

            private Action action;
            public MyAction(Action action) {
                this.action = action;
            }

            protected override ActionResult<bool> ActionImplemetation()
            {
                action.Invoke();
                return true;
            }

            protected override bool AuthorizeImplementation()
            {
                return true;
            }
        }


        public OutputType ExecuteAction<ActionType, InputType, OutputType>(InputType input) where ActionType : BaseMysteryAction<InputType, OutputType>, new() {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var warping_action = new MyAction<InputType, OutputType, ActionType>();
                Assert.IsTrue(executor.executeAction(warping_action, input).isSuccessfull);
                return warping_action.action_result;
            }
        }


        public void doIt(Action action) {
            using (WebActionExecutor executor = new WebActionExecutor())
            {
                var warping_action = new MyAction(action);
                Assert.IsTrue(executor.executeAction(warping_action).isSuccessfull);
            }
        }

    }
}
