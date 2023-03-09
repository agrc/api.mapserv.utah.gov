namespace AGRC.api.Models.Configuration {
    // TODO Duplicated in developer.mapserv.utah.gov
    public class DatabaseConfiguration {
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "6379";
        public string ConnectionString =>
            $"{Host}:{Port},abortConnect=false";
    }
}
