using Mystery.Configuration;
using Mystery.Json;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Messaging
{
    [GlobalAvalibleObjectImplementation(
        implementation_of = typeof(INotifier),
        overrides_exsisting = true,
        singleton = true)]
    public class DefaultNotifier : NotifierChain
    {

        public DefaultNotifier():base(getChain()) {

        }

        
        private static IEnumerable<INotifier> getChain() {
            object helper = new object();
            //do we have mongodb available?
            //if yes we will store there to
            var notifiers = new List<INotifier>();
            if (helper.getGlobalObject<MysteryMongoDb>()
                .local_db != null) {
                notifiers.Add(new MongoMockNotifier());
            }
            var send_grid_conf = helper.getGlobalObject<IConfigurationProvider>()
                .getConfiguration<SendGridNotifierConfig>();
            //send grind available?
            if (send_grid_conf.enabled &&!string.IsNullOrEmpty(send_grid_conf.api_key)) {
                notifiers.Add(new SendGridNotifier(send_grid_conf));
            }

            return notifiers;

        }
    }
}
