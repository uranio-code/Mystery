using Mystery.Content;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.UI
{
    public interface IMysteryUrl
    {
        string url { get; }
    }

    public class MysteryUrls {
        static public IMysteryUrl StartPage = new MysteryUrl("");
    }

    public class MysteryUrl : IMysteryUrl
    {
        private string _url;
        public MysteryUrl(string url) {
            _url = url;
        }
        public string url
        {
            get
            {
                return _url;
            }
        }
    }

    public class ContentUrl : IMysteryUrl
    {
        private string _url;
        public ContentUrl(IContent content) {
            Type type = content.GetType();
            if (type.getMysteryAttribute<ContentView>() != null)
            {
                ContentType ct = type.getMysteryAttribute<ContentType>();
                _url = ct.name + '/' + content.guid.Tiny();
            }
            else
                _url = string.Empty;
        }
        public string url
        {
            get
            {
                return _url;
            }
        }
    }

    public class ContentTypeUrl : IMysteryUrl
    {
        private string _url;
        public ContentTypeUrl(Type ContentType)
        {
            if (ContentType.getMysteryAttribute<ContentTypeView>() != null)
            {
                ContentType ct = ContentType.getMysteryAttribute<ContentType>();
                _url = "Type/" + ct.name;
            }
            else
                _url = string.Empty;
        }
        public string url
        {
            get
            {
                return _url;
            }
        }
    }
}
