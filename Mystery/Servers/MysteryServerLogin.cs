using Mystery.Configuration;
using Mystery.Content;
using Mystery.MysteryAction;
using Mystery.Register;
using Mystery.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Servers
{
    public class MysteryServerLoginInput
    {
        public string request_guid { get; set; }
        public Byte[] signature { get; set; }
    }

    [PublishedAction(
        input_type: typeof(MysteryServerLoginInput),
        output_type: typeof(MysteryServer),
        url = nameof(MysteryServerLogin))]
    public class MysteryServerLogin :
        BaseMysteryAction<MysteryServerLoginInput,
            MysteryServer>, ICanRunWithOutLogin
    {
        protected override ActionResult<MysteryServer> ActionImplemetation()
        {
            var cd = this.getGlobalObject<IContentDispatcher>();
            var guid = input.request_guid.fromTiny();
            var request = cd.GetContent<MysteryServerAutheticationRequest>(guid);
            if(request == null)
                return ActionResultTemplates<MysteryServer>.InvalidInput;
            var server = request.server.value;
            var rsa = server.getRsa();
            var good = rsa.VerifyData(input.request_guid.getBytes(),
                new SHA1CryptoServiceProvider(), input.signature);
            if(!good)
                return ActionResultTemplates<MysteryServer>.InvalidInput;

            this.getGlobalObject<MysterySession>().authenticated_user = server;
            return server;

        }

        protected override bool AuthorizeImplementation()
        {
            var conf = this.getGlobalObject<IConfigurationProvider>().
                getConfiguration<MysteryServerConf>();
            return conf.allow_server_login;
        }
    }
}
