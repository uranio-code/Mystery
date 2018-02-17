using Mystery.Content;
using Mystery.Register;
using System.Linq;

namespace Mystery.Messaging
{

    /// <summary>
    /// This class implements the ICodifiedMEssage interface.
    /// Give the code and the language it simply retrieves the text from the database.
    /// Please be aware that the caller has the full responsibility replace the dynamic part(s) 
    /// of the message.body with the proper text.
    /// </summary>
    public class BaseCodifiedMessage : ICodifiedMessage
    {
        public string code { get; set; }

        public string language { get; set; }

        private IUserMessage _message;
        public IUserMessage message
        {
            get
            {
                if (_message != null) return _message;

                // get it from the database
                IContentDispatcher cd = this.getGlobalObject<IContentDispatcher>();

                BaseCodifiedMessageTemplate _template = cd.GetAllByFilter<BaseCodifiedMessageTemplate>(x => x.code == code || x.language == language).FirstOrDefault();

                // not found
                if (_template == null) return _message;

                _message = new BaseMessage();
                _message.body = _template.template;

                return _message;
            }
        }
    }
}
