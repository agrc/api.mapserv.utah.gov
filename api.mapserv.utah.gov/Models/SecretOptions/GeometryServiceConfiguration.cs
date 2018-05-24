namespace api.mapserv.utah.gov.Models.SecretOptions
{
    public class GeometryServiceConfiguration
    {
        public string Host { get; set; }
        public string Url { get; set; }

        public string GetUrl()
        {
            return Url.Replace("{Host}", Host);
        }
    }
}
