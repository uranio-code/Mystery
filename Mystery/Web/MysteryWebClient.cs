using System;
using System.Net;
using Mystery.Register;

namespace Mystery.Web
{
    [GlobalAvalibleObjectImplementation(singleton =true)]
    public class MysteryWebClient : WebClient
    {
        private readonly CookieContainer m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = m_container;
            }
            return request;
        }
    }
}
