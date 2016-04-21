namespace WebAPI.Common.Models.Raven.Admin
{
    public class AdminContainer
    {
        public AdminContainer(string[] emails)
        {
            Emails = emails;
        }

        public string[] Emails { get; set; }
    }
}