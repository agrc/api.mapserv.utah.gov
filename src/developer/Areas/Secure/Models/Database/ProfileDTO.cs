using System;
namespace developer.mapserv.utah.gov.Areas.Secure.Models.Database
{
    public class ProfileDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Company { get; set; }
        public string JobCategory { get; set; }
        public string JobTitle { get; set; }
        public int Experience { get; set; }
        public string ContactRoute { get; set; }
        public bool Confirmed { get; set; }
    }
}
