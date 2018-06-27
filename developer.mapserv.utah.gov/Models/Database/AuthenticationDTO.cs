using System;
namespace developer.mapserv.utah.gov.Models.Database
{
    public class AuthenticationDTO
    {
        public string Password
        {
            get;
            set;
        }

        public string Salt
        {
            get;
            set;
        }

        public byte[] GetSaltBytes(string salt) {
            return Convert.FromBase64String(salt);
        }

        public bool IsValid(){
            return !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Salt);
        }
    }
}
