using AnglicanGeek.MarkdownMailer;
using WebAPI.Common.Abstractions;

namespace WebAPI.Dashboard.Commands.Email
{
    public class SendPasswordResetEmailCommand : Command
    {
        public SendPasswordResetEmailCommand(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        protected override void Execute()
        {
            var mailSender = new MailSender();
            mailSender.Send("noreply@utah.gov", Email, "AGRC Web API Password Reset",
                            @"AGRC Web API
-

We have receieved a request to reset your password. Please log in and update your password. " +
                            string.Format(@"Your new password is 

#{0}", Password));
        }

        public override string ToString()
        {
            return string.Format("Email: {0}, Password: {1}", Email, Password);
        }
    }
}