namespace api.mapserv.utah.gov.Models.Configuration {
    public abstract class UrlConfigurationBase {
        public virtual string Protocol { get; set; } = "http";
        public virtual string Host { get; set; } = "localhost";
        public virtual string Port { get; set; } = "80";

        public virtual string GetHost() {
            var host = Host;

            if (Port != "80") {
                host = $"{host}:{Port}";
            }

            return $"{Protocol}://{host}";
        }

        public abstract string Url();
    }
}
