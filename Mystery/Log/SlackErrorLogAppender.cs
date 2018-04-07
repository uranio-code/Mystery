using log4net.Appender;
using log4net.Core;
using Mystery.Json;
using Mystery.Register;
using Mystery.Slack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Log
{
    public class SlackErrorLogAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var sm = this.getGlobalObject<SlackMessager>();
            var converter = this.getGlobalObject<MysteryJsonConverter>();

            var json = converter.getJson(loggingEvent);
            sm.SendMessage("error occured");
            sm.SendMessage(loggingEvent.RenderedMessage);
        }
    }
}
