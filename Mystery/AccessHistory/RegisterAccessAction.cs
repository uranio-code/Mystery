using Mystery.Json;
using Mystery.Register;
using Mystery.MysteryAction;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.AccessHistory
{
    public class RegisterAccessInput
    {
        public string tiny_guid { get; set; }

        public string content_type_name { get; set; }
    }
    public class RegisterAccessAction : BaseMysteryAction<RegisterAccessInput, bool>, ICanRunWithOutLogin
    {
        protected override ActionResult<bool> ActionImplemetation()
        {
            Guid parsed = input.tiny_guid.fromTiny();
            if (parsed == Guid.Empty && !Guid.TryParse(input.tiny_guid, out parsed))
                return ActionResultTemplates<bool>.InvalidInput;

            var session = this.getGlobalObject<MysterySession>();


            this.getGlobalObject<UserAccessHistory>().collection.InsertOne(
                new AccessRecord()
                {
                    content_guid = parsed,
                    user_guid = session.authenticated_user == null ? Guid.Empty : session.authenticated_user.guid,
                    session_id = session.id,
                    entry_date = DateTime.Now,
                    content_type_name = input.content_type_name,
                });
            return true;
        }

        protected override bool AuthorizeImplementation()
        {
            return true;
        }
    }
}


