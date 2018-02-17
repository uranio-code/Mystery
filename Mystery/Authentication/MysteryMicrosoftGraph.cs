using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Mystery.Register;
namespace Mystery.Authentication
{
    public class MysteryAuthenticationProvider : IAuthenticationProvider
    {
        private string _token;

        public MysteryAuthenticationProvider(string token) {
            _token = token;
        }
        public MysteryAuthenticationProvider(LiveIdAccessTokenInfo token)
        {
            token = this.getGlobalObject<MicrosowftTokenCache>().getFreshToken(token);
            _token = token.access_token;
        }

        public Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", "Bearer " + _token);
            return Task.FromResult(0);
        }
    }
   
}
