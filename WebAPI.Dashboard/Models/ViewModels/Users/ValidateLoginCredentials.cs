namespace WebAPI.Dashboard.Models.ViewModels.Users
{
    /// <summary>
    ///     A transfer object for validing a user
    /// </summary>
    public class ValidateLoginCredentials
    {
        public ValidateLoginCredentials(string password, string salt, string id)
        {
            Password = password;
            Salt = salt;
            Id = id;
        }

        public string Password { get; set; }
        public string Salt { get; set; }
        public string Id { get; set; }
    }
}