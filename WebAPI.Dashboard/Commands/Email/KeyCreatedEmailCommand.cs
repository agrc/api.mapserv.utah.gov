using AnglicanGeek.MarkdownMailer;
using WebAPI.Common.Abstractions;
using WebAPI.Common.Models.Raven.Keys;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Commands.Email
{
    public class KeyCreatedEmailCommand : Command
    {
        public KeyCreatedEmailCommand(Account account, ApiKey newKey)
        {
            Account = account;
            NewKey = newKey;
        }

        public Account Account { get; set; }
        public ApiKey NewKey { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Account: {1}, NewKey: {2}", "NewKeyCreatedEmailCommand", Account, NewKey);
        }

        protected override void Execute()
        {
            var mailSender = new MailSender();
            mailSender.Send("noreply@utah.gov", App.NotifyList, "AGRC Web API New Key Notification",
                            @"# New API Key Generated

Please make sure this key is *awesome*.

" + Account + @"

"
  + NewKey);
        }
    }
}