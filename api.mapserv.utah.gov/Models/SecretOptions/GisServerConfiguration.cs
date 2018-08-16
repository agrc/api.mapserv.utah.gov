namespace api.mapserv.utah.gov.Models.SecretOptions
{
    public class GisServerConfiguration
    {
        public string Protocol { get; set; } = "http";
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "80";

        public override string ToString()
        {
            var host = Host;

            if (Port != "80")
            {
                host = $"{host}:{Port}";
            }

            return $"{Protocol}://{host}";
        }
    }
}
