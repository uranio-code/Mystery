using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Mystery.Web;
using Mystery.Register;
using Mystery.Configuration;
using Mystery.Json;

namespace MysteryWebLogic.Languanges
{


    public class LanguagesDictionaries
    {
        public Dictionary<string,object> data { get; set; }

        public DateTime download_date { get; set; }

        private static bool downloaded = false;
        private static object _lock = new object();

        private bool needDownload() {
            //we download it at each run
            if (!downloaded) return true;
            //or if we never did
            if (this.download_date == DateTime.MinValue) return true;
            //or if we didn't get anything
            if (this.data == null) return true;
            return false;
        }

        public void ensure() {
            if (!needDownload())
                return;

            lock (_lock) {

                if (!needDownload())
                    return;
                //we lost this implementation, we will need to replace it with something else
                return;
                var previous = ServicePointManager.ServerCertificateValidationCallback;
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback =
                    (object sender, X509Certificate certificate,
                    X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                    var c = this.getGlobalObject<MysteryWebClient>();
                    var dicts = c.DownloadString("https://calean.cloudapp.net/Mystery/LanguageService/Dictionaries?root_name=main");
                    var converter = this.getGlobalObject<MysteryJsonConverter>();
                    this.data = converter.readJson<Dictionary<string,object>>(dicts);
                    this.download_date = DateTime.Now;
                    downloaded = true;
                    var conf = this.getGlobalObject<IConfigurationProvider>();
                    conf.setConfiguration(this);

                }
                catch (Exception ex)
                {
                    this.log().Error(ex);
                }
                finally
                {
                    ServicePointManager.ServerCertificateValidationCallback = previous;
                }
            }
            
            

        }
    }
}
