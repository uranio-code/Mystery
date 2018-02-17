using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Authentication
{
    [GlobalAvalibleObjectImplementation(singleton = true)]
    public class MicrosowftTokenCache
    {

        private IDictionary<string, LiveIdAccessTokenInfo> _cache = new Dictionary<string, LiveIdAccessTokenInfo>();
        private IDictionary<string, DateTime> _expire = new Dictionary<string, DateTime>();

        public LiveIdAccessTokenInfo getFreshToken(LiveIdAccessTokenInfo given) {
            //first let's see we have one in cache
            var account_info = given.getAccountInfo();
            var result = _cache.ContainsKey(account_info?.email) ? _cache[account_info.email] : given;
            var expiration_date = _expire.ContainsKey(account_info?.email) ? _expire[account_info.email] : DateTime.Now.AddMinutes(-1);
            if (expiration_date < DateTime.Now) {
                result = LiveIdAccessTokenInfo.Refresh(result.refresh_token);
                _cache[account_info.email] = result;
                _expire[account_info.email] = DateTime.Now.AddMinutes(30);
            }
            return result;
        }
        public LiveIdAccessTokenInfo getToken(string email) {
            var result = _cache.ContainsKey(email) ? _cache[email] : null;
            var expiration_date = _expire.ContainsKey(email) ? _expire[email] : DateTime.Now.AddMinutes(-1);
            if (result != null && expiration_date > DateTime.Now)
                return result;
            else
                return null;
        }
    }
}
