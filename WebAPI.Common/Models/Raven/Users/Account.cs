using System;

namespace WebAPI.Common.Models.Raven.Users
{
    public class Account : AccountAccessBase
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string JobCategory { get; set; }
        public string JobTitle { get; set; }
        public int CodingExperience { get; set; }
        public string ContactRoute { get; set; }
        public EmailConfirmation Confirmation { get; set; }
        public KeyQuota KeyQuota { get; set; }
        public PasswordHashAndSalt Password { get; set; }

        public string Name
        {
            get
            {
                var name = string.Format("{0} {1}", FirstName, LastName).Trim();

                if (name.Length > 20)
                {
                    return name.Substring(0, 20) + "...";
                }

                return name;
            }
        }

        public void ResetConfirmation(string key)
        {
            Confirmation.Key = key;
            Confirmation.Confirmed = false;
            Confirmation.ConfirmationDate = new DateTime();
            Confirmation.TimesSent = 0;
        }

        public override string ToString()
        {
            return string.Format(@"## User
* **Name**: {0}
* **Email**: {1}
* **Contact Route**: {2}", Name, Email, ContactRoute);
        }
    }
}