using System.Net.Mail;
using AnglicanGeek.MarkdownMailer;
using WebAPI.Common.Abstractions;

namespace WebAPI.API.Commands.Geocode
{
    public class NotifyGeocoderDownCommand : Command
    {
        public NotifyGeocoderDownCommand(string message, string toAddresses)
        {
            Message = message;
            ToAddresses = toAddresses;
        }

        /// <summary>
        ///     Gets or sets the message.
        /// </summary>
        /// <value>
        ///     The message about which locator is down.
        /// </value>
        protected string Message { get; set; }

        /// <summary>
        ///     Gets or sets to addresses.
        /// </summary>
        /// <value>
        ///     To addresses. "," separated for multiple
        /// </value>
        protected string ToAddresses { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, Message: {1}, ToAddresses: {2}", "NotifyGeocoderDownCommand", Message,
                                 ToAddresses);
        }

        protected override void Execute()
        {
            var mail = new MailMessage();
            mail.To.Add(ToAddresses);
            mail.From = new MailAddress("noreply@utah.gov", "Web API");
            mail.Subject = string.Format("AGRC Web API - {0}", Message);
            mail.Body =
                string.Format(
                    @"AGRC Web API
-

## A geocoder is down. 

> *{0}* 

Check all of the other ones as well.",
                    Message);

            var mailSender = new MailSender();
            mailSender.Send(mail);
        }
    }
}