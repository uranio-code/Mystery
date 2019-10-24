using Mystery.Configuration;
using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Servers
{
    public class GetMysteryServerAutheticationRequestInput
    {
        public string rsa { get; set; }
    }
    [PublishedAction(
        input_type: typeof(GetMysteryServerAutheticationRequestInput),
        output_type: typeof(MysteryServerAutheticationRequest),
        url = nameof(GetMysteryServerAutheticationRequest))]
    public class GetMysteryServerAutheticationRequest :
        BaseMysteryAction<GetMysteryServerAutheticationRequestInput,
            MysteryServerAutheticationRequest>, ICanRunWithOutLogin
    {
        protected override ActionResult<MysteryServerAutheticationRequest> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var cc = this.getGlobalObject<IGlobalContentCreator>();
            var server = cd.GetAllByFilter<MysteryServer>((x) => x.rsa == input.rsa).FirstOrDefault();
            if (server == null) {
                var conf = this.getGlobalObject<IConfigurationProvider>().
                getConfiguration<MysteryServerConf>();
                if (!conf.allow_auto_creation)
                    return ActionResultTemplates<MysteryServerAutheticationRequest>.UnAuthorized;
                server = cc.getAndAddNewContent<MysteryServer>();
                server.rsa = input.rsa;
                cd.Add(server);
            }
            //server shall have only 1 open request at time
            //so we delete all the open ones
            var open_requests = cd.GetAllByFilter<MysteryServerAutheticationRequest>((x) => x.server.guid == server.guid);
            cd.RemoveContents(open_requests);
            //and we create a new one
            var result = cc.getAndAddNewContent<MysteryServerAutheticationRequest>();
            result.server = server;
            cd.Add(result);

            return result;


        }

        protected override bool AuthorizeImplementation()
        {
            var conf = this.getGlobalObject<IConfigurationProvider>().
                getConfiguration<MysteryServerConf>();
            return conf.allow_server_login;
        }
    }
}
