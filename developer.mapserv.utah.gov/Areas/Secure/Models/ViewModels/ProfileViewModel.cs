using System;
using System.ComponentModel.DataAnnotations;

namespace developer.mapserv.utah.gov.Areas.Secure.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(512)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(128)]
        public string First { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(128)]
        public string Last { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(512)]
        public string Company { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(256)]
        public string JobCategory { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(256)]
        public string JobTitle { get; set; }

        [DataType(DataType.Text)]
        public int Experience { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(128)]
        public string ContactRoute { get; set; }
    }
}
