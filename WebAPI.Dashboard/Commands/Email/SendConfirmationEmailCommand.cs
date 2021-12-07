using AnglicanGeek.MarkdownMailer;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Commands.Email
{
    public class SendConfirmationEmailCommand : Command
    {
        public SendConfirmationEmailCommand(Account account, string url)
        {
            Account = account;
            Url = url;
        }

        public Account Account { get; set; }

        public string Url { get; set; }

        protected override void Execute()
        {
            var mailSender = new MailSender();
            mailSender.Send("noreply@utah.gov", Account.Email, "UGRC Web API Email Confirmation",
                            @"UGRC Web API
-

Thank you for applying for registration with the UGRC Web API. We have received your request and we will allow you to" +
                            string.Format(" generate API keys as soon as you [confirm your email address]({0}) ", Url) +
                            string.Format(@"by navigating to the following url: 

{0}", Url));

            Account.Confirmation.TimesSent++;
        }

        public override string ToString()
        {
            return string.Format("{0}, Account: {1}", "SendConfirmationEmail", Account);
        }
    }
}