using System;
using System.Threading.Tasks;
using Mystery.Register;
using Mystery.Configuration;
using SendGrid.Helpers.Mail;
using Mystery.Users;
using SendGrid;

namespace Mystery.Messaging
{

    /// <summary>
    /// class used to store the api key in mystery configuration provider
    /// </summary>
    public class SendGridNotifierConfig {
        public string api_key { get; set; }
        public string email_source_name { get; set; }
        public string email_source_address { get; set; }

        public bool enabled { get; set; }
    }

    public class SendGridNotifier : INotifier
    {

        private SendGridNotifierConfig _config;
        private SendGridClient _api;

        public SendGridNotifier(SendGridNotifierConfig config) {
            if (config == null) {
                config = this.getGlobalObject<IConfigurationProvider>().getConfiguration<SendGridNotifierConfig>();
            }
            _api = new SendGridClient(config.api_key);
            _config = config;
        }

        public void sendMessage(IUserMessage message)
        {
            if (message == null)
                return;
            Execute(message).Wait();
        }

        async Task Execute(IUserMessage message)
        {
            try
            {
                EmailAddress from = new EmailAddress(_config.email_source_address,_config.email_source_name);
                String subject = message.title;

                Personalization personalization = new Personalization();
                foreach (var receiver in message.to) {
                    if (receiver == null) continue;
                    personalization.Tos.Add(new EmailAddress(receiver.email, receiver.fullname));
                }
                foreach (var receiver in message.cc)
                {
                    if (receiver == null) continue;
                    personalization.Ccs.Add(new EmailAddress(receiver.email, receiver.fullname));
                }
                var mail = new SendGridMessage();
                var body = message.body;
                if (message.from != null && message.from.account_type == UserType.normal) {
                    //TBD translation
                    body = "message from: " + message.from.ReferenceText + "<br/>" + Environment.NewLine + message.body; ;
                }
                mail.Contents.Add(new SendGrid.Helpers.Mail.Content("text/html", body));
                mail.From = from;
                mail.Subject = subject;
                mail.Personalizations.Add(personalization);

                Response response = await _api.SendEmailAsync(mail);
                Console.WriteLine(response.StatusCode);
                Console.WriteLine(response.Body.ReadAsStringAsync().Result);
                Console.WriteLine(response.Headers.ToString());
            }
#pragma warning disable CS0168 // Variable is declared but never used, suppressed because it is convenient in debug
            catch(Exception ex) {
#pragma warning restore CS0168 // Variable is declared but never used

            }

            

        }

    }
}
