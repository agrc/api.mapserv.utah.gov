using System.ComponentModel.DataAnnotations;

namespace WebAPI.Common.Models.Raven.Users
{
    public class AccountAccessBase
    {
        public string Id { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
    }
}