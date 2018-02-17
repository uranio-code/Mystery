using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Messaging
{
    /// <summary>
    /// provide a simple way to have more than 1 notifier
    /// </summary>
    public class NotifierChain:INotifier
    {
        private IEnumerable<INotifier> _notifiers;
        public NotifierChain(IEnumerable<INotifier> notifiers) {
            if (notifiers == null) {
                _notifiers = new List<INotifier>();
            }
            else {
                _notifiers = new List<INotifier>(notifiers);
            }
            
        }

        public void sendMessage(IUserMessage message)
        {
            if (message == null)
                return;
            foreach (var notifier in _notifiers) {
                notifier.sendMessage(message);
            }
        }
    }
}
