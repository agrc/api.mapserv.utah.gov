using System.ComponentModel.DataAnnotations;
using WebAPI.Common.Models.Raven.Users;

namespace WebAPI.Dashboard.Models.ViewModels.Users
{
    public class LoginCredentials : AccountAccessBase
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string Token { get; set; }
    }
}