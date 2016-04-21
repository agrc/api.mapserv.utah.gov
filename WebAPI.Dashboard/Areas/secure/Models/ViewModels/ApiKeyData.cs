using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Areas.secure.Models.ViewModels
{
    public class ApiKeyData
    {
        public string UrlPattern { get; set; }
        public string Ip { get; set; }
        public ApiKey.ApplicationStatus AppStatus { get; set; }

        public override string ToString()
        {
            return string.Format("UrlPattern: {0}, Ip: {1}, AppStatus: {2}", UrlPattern, Ip, AppStatus);
        }
    }
}