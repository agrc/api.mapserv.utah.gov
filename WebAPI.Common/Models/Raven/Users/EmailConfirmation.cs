using System;

namespace WebAPI.Common.Models.Raven.Users
{
    public class EmailConfirmation
    {
        public EmailConfirmation(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Email confirmation key cannot be empty.");
            }

            Key = key;
        }

        public string Key { get; set; }
        public bool Confirmed { get; set; }
        public int TimesSent { get; set; }
        public DateTime ConfirmationDate { get; set; }
    }
}