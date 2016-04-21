namespace WebAPI.Common.Models.Raven.Users
{
    public class PasswordHashAndSalt
    {
        public PasswordHashAndSalt(string password, string salt)
        {
            HashedPassword = password;
            Salt = salt;
        }

        public string HashedPassword { get; set; }
        public string Salt { get; set; }

        public override string ToString()
        {
            return string.Format("HashedPassword: {0}, Salt: {1}", HashedPassword, Salt);
        }
    }
}