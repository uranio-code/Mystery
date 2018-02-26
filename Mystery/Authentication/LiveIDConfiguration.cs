using Mystery.Configuration;
using Mystery.Register;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace Mystery.Authentication
{
    public class LiveIDAccountInfo
    {
        public string name { get; set; }
        public string email { get; set; }
        public string preferred_username { get; set; }
    }



    public delegate void LiveIdAccessTokenInfoReceivedHandler(LiveIdAccessTokenInfo live_id_info);

    public class LiveIdAccessTokenInfo
    {
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public string access_token { get; set; }

        public static event LiveIdAccessTokenInfoReceivedHandler LiveIdAccessTokenInfoReceived;
        public static LiveIdAccessTokenInfo Aquire(string code, LiveIDConfiguration liveIDconf) {
            if (string.IsNullOrEmpty(code))
                return null;

            var secret = code.getGlobalObject<IConfigurationProvider>().getConfiguration<LiveIDSecret>();

            var c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("client_id", liveIDconf.clientId);
            reqparm.Add("redirect_uri", liveIDconf.redirectUri);
            reqparm.Add("client_secret", secret.client_secret);
            reqparm.Add("code", code);
            reqparm.Add("grant_type", "authorization_code");

            byte[] responsebytes = c.UploadValues("https://login.microsoftonline.com/common/oauth2/v2.0/token", "POST", reqparm);
            string json = responsebytes.getString();


            LiveIdAccessTokenInfo access_token_info = Newtonsoft.Json.JsonConvert.DeserializeObject<LiveIdAccessTokenInfo>(json);
            if (!string.IsNullOrWhiteSpace(access_token_info.access_token))
                LiveIdAccessTokenInfoReceived?.Invoke(access_token_info);
            return access_token_info;
        }
        public static LiveIdAccessTokenInfo Refresh(string refresh_token)
        {
            if (string.IsNullOrEmpty(refresh_token))
                return null;

            var liveIDconf = refresh_token.getGlobalObject<IConfigurationProvider>().getConfiguration<LiveIDConfiguration>();
            var secret = refresh_token.getGlobalObject<IConfigurationProvider>().getConfiguration<LiveIDSecret>();

            var c = new WebClient();
            c.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var reqparm = new System.Collections.Specialized.NameValueCollection();
            reqparm.Add("client_id", liveIDconf.clientId);
            reqparm.Add("scope", liveIDconf.scopes);
            reqparm.Add("client_secret", secret.client_secret);
            reqparm.Add("refresh_token", refresh_token);
            reqparm.Add("grant_type", "refresh_token");

            byte[] responsebytes = c.UploadValues("https://login.microsoftonline.com/common/oauth2/v2.0/token", "POST", reqparm);
            string json = responsebytes.getString();

            LiveIdAccessTokenInfo access_token_info = Newtonsoft.Json.JsonConvert.DeserializeObject<LiveIdAccessTokenInfo>(json);
            return access_token_info;
        }

        public LiveIDAccountInfo getAccountInfo() {
            var payload = JwtPayload.Base64UrlDeserialize(this.id_token.Split('.')[1]);
            var json = payload.SerializeToJson();
            LiveIDAccountInfo account_info = Newtonsoft.Json.JsonConvert.DeserializeObject<LiveIDAccountInfo>(json);
            return account_info;
        }

    }

    public class LiveIDConfiguration
    {
        public string clientId { get; set; } 
        public string scopes { get; set; } = "wl.signin onedrive.appfolder wl.emails";
        public string redirectUri { get; set; }

        public bool allow_new_users { get; set; } = false; //top level ability of creating new accounts


    }

    public class LiveIDSecret
    {
        public string client_secret { get; set; }
    }
}
