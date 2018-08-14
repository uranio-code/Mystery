using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mystery.Jira
{
    public class JiraConsumer
    {

        private static InMemoryTokenManager TokenManager = new InMemoryTokenManager("carlo", "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAL/G49xy3PfulULEE5TvBRCJa0yqj7Pkw1Rv8XPCfbSqyKNYgGgwtjX+Cx3ODrjX+3CkXl26CqHTplNdeUVrjQV0SStInqVokhMR/Ffou63jcxC9fskDTjh6Q9dUXEXkfl+iycvRduv5a9HV29xk7l/RojHwwdg70BGFNc+Dp17BAgMBAAECgYBvxsbMmI4+W+rXbV1UczPqfY3ys37qhS1sK4r5w6RtBxXGTzEYiTvKoHVuO8nM5uYAs2zlCChmRewtrs+eLJ+WSeOCWh82TVUBYFBX6jT0l8TtAYnrqUvFLhTIlhgeudmxOyByxOaj1zN4JwLQ3hKXSzbws7/HKTz/kwRUY372gQJBAOKjc8OYzHsqxmhSfw0wNPMUbJtqLi6uz06ChfVJZpN9Csn66i7VqoDIKWSrF9jTABhcvD3w13G17yyhgR8mRxkCQQDYnz2/iJXq+2g9MCN/+jI6k5uhyT2IGp2ickjJe2q0BD8IipbpyA4OH6JrrM7IKOcTuAlPdsMm2wltnE/y2BHpAkBinecQlFtduMYuvL7mGTThFOERW0mPLKeuLONnUTIeOUnJi7H9ASI4+V/xmvU8dsvSzf9nIHZO13CkRf5udHR5AkARtcMom5QWKAO3rO3aEqx0mYjJy1gafoKQu2M0BcENgqNcoWgBKPftM6zlvLUTLDToKc2pD36Y+KNmsLbglcWxAkAid6xg76rq8EKzGFkd44Gx0x4F2LKQw1URbblAJ4MVYtJjenRaogZwhM27JB8jtSbOIVTiCNecvt5kH9QqDox8");

        private static readonly ServiceProviderDescription ServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://jira.iter.org/plugins/servlet/oauth/request-token", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://jira.iter.org/plugins/servlet/oauth/authorize'", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://jira.iter.org/plugins/servlet/oauth/access-token", HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
        };

        private string AccessToken;

        internal static IEnumerable<long> GetIndividualFlags(Enum flags)
        {
            long flagsLong = Convert.ToInt64(flags);
            for (int i = 0; i < sizeof(long) * 8; i++)
            { // long is the type behind the largest enum
              // Select an individual application from the scopes.
                long individualFlagPosition = (long)Math.Pow(2, i);
                long individualFlag = flagsLong & individualFlagPosition;
                if (individualFlag == individualFlagPosition)
                {
                    yield return individualFlag;
                }
            }
        }

        internal static Uri GetCallbackUrlFromContext()
        {
            Uri callback = MessagingUtilities.GetRequestUrlFromContext().StripQueryArgumentsWithPrefix("oauth_");
            return callback;
        }

        /// <summary>
		/// Requests authorization from Google to access data from a set of Google applications.
		/// </summary>
		/// <param name="consumer">The Google consumer previously constructed using <see cref="CreateWebConsumer"/> or <see cref="CreateDesktopConsumer"/>.</param>
		/// <param name="requestedAccessScope">The requested access scope.</param>
		public static void RequestAuthorization(WebConsumer consumer )
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            //var extraParameters = new Dictionary<string, string> {
            //    { "scope", GetScopeUri(requestedAccessScope) },
            //};
            Uri callback = GetCallbackUrlFromContext();
            var request = consumer.PrepareRequestUserAuthorization(callback, new Dictionary<string, string>(), null);
            consumer.Channel.Send(request);
        }

        public void doSomething() {
            var jira = new WebConsumer(ServiceDescription, TokenManager);
            var accessTokenResponse = jira.ProcessUserAuthorization();
            if (accessTokenResponse != null)
            {
                this.AccessToken = accessTokenResponse.AccessToken;
            }
            else if (this.AccessToken == null)
            {
                // If we don't yet have access, immediately request it.
                RequestAuthorization(jira);
            }

        }

    }
}
