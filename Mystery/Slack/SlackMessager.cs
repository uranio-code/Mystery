using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mystery.Configuration;
using Mystery.Json;
using Mystery.Register;
using Mystery.Web;

namespace Mystery.Slack
{

    public class SlackMessage {
        public string text { get; set; }
    }

    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class SlackMessager
    {
        public bool SendMessage(string message) {
            var cf = this.getGlobalObject<IConfigurationProvider>();
            var conf = cf.getConfiguration<SlackConfiguration>();
            if (string.IsNullOrWhiteSpace(conf.Webhook_URL))
                return false;
            var c = new WebClient();
            var slack_message = new SlackMessage() { text = message };
            var jc = this.getGlobalObject<MysteryJsonConverter>();
            var json = jc.getJson(slack_message);
            var result = c.UploadString(conf.Webhook_URL, json);
            return true;
        }
    }
}
