using WebAPI.Common.Models.Raven.Keys;

namespace WebAPI.Dashboard.Areas.secure.Models.ViewModels
{
    public class ApiKeyData
    {
        public string UrlPattern { get; set; }
        public string Ip { get; set; }
        public string AgolOrganization { get; set; }
        public ApiKey.ApplicationStatus AppStatus { get; set; }

        public override string ToString()
        {
            return $"UrlPattern: {UrlPattern}, Ip: {Ip}, AppStatus: {AppStatus}, AgolOrganization: {AgolOrganization}";
        }
    }
}