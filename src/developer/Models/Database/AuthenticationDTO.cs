using System;
namespace developer.mapserv.utah.gov.Models.Database
{
    public class AccountIdDTO
    {
        public int Id { get; set; }
    }

    public class AuthenticationDTO : AccountIdDTO
    {
        public string Password { get; set; }

        public string Salt { get; set; }

        public byte[] GetSaltBytes(string salt) => Convert.FromBase64String(salt);

        public bool IsValid() => !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Salt);
    }
}
