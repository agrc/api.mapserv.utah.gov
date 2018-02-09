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
            return $"NotifyGeocoderDownCommand, Message: {Message}, ToAddresses: {ToAddresses}";
        }

        protected override void Execute()
        {
            var mail = new MailMessage();
            mail.To.Add(ToAddresses);
            mail.From = new MailAddress("noreply@utah.gov", "Web API");
            mail.Subject = $"AGRC Web API - {Message}";
            mail.Body = $@"AGRC Web API
-

## A geocoder is down. 

> *{Message}* 

Check all of the other ones as well.";

            var mailSender = new MailSender();
            mailSender.Send(mail);
        }
    }
}