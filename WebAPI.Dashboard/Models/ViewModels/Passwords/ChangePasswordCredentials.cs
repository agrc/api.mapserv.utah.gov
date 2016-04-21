using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dashboard.Models.ViewModels.Passwords
{
    public class ChangePasswordCredentials
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }
    }
}