using Mystery.Encryption;
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
    /// <summary>
    /// client to connect to other mystery instances
    /// </summary>
    public class MysteryServerClient: MysteryWebClient
    {
        private string _url { get; set; }
        public MysteryServerClient(string url) {
            _url = url;
            var input = new GetMysteryServerAutheticationRequestInput();
            var my_rsa = this.getGlobalObject<IMyRsaProvider>().getMySra();
            input.rsa = my_rsa.ToXmlString(false);
            var auth_request = Invoke<GetMysteryServerAutheticationRequestInput,
                MysteryServerAutheticationRequest,
                GetMysteryServerAutheticationRequest>(input);
            var bytes = auth_request.guid.Tiny().getBytes();
            var login_input = new MysteryServerLoginInput();
            login_input.request_guid = auth_request.guid.Tiny();
            login_input.signature = my_rsa.SignData(
                login_input.request_guid.getBytes(),
                new SHA1CryptoServiceProvider());
            var me =Invoke<MysteryServerLoginInput,
                MysteryServer,
                MysteryServerLogin>(login_input);
            if (me == null)
                throw new Exception("could not login");
        }


        public ResultType Invoke<InputType, ResultType, ActionType>(InputType input) where ActionType : BaseMysteryAction<InputType, ResultType>{
            var action_url = _url + "/" + typeof(ActionType).
                getMysteryAttribute<PublishedAction>().url;
            var converter = this.getGlobalObject<Json.MysteryJsonConverter>();
            var json = converter.getJson(input);
            var result_json = this.UploadString(action_url, json);
            var action_result = converter.readJson<WebActionResult>(result_json);
            var result = converter.readJson<ResultType>(action_result.json_output);
            return result;
        }
    }
}
