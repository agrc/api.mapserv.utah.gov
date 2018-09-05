using System;
using System.ComponentModel.DataAnnotations;
using developer.mapserv.utah.gov.Areas.Secure.Models.Database;

namespace developer.mapserv.utah.gov.Areas.Secure.Models.ViewModels
{
    public class ProfileViewModel
    {
        public ProfileViewModel(ProfileDTO dto)
        {
            Email = dto.Email;
            First = dto.First;
            Last = dto.Last;
            Company = dto.Company;
            JobTitle = dto.JobTitle;
            JobCategory = dto.JobCategory;
            Experience = dto.Experience;
            ContactRoute = dto.ContactRoute;
            Confirmed = dto.Confirmed;
        }

        [Required]
        [EmailAddress]
        [MaxLength(512)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; }

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

        public bool Confirmed { get; }
    }
}
