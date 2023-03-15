namespace AGRC.api.Features.Searching {
    // TODO Duplicated in developer.mapserv.utah.gov
    public class SearchProviderConfiguration {
        public string Host { get; set; } = "opensgid.agrc.utah.gov";
        public string Db { get; set; } = "opensgid";
        public string Username { get; set; } = "agrc";
        public string Password { get; set; } = "agrc";

        public string ConnectionString =>
            $"Host={Host};Username={Username};Password={Password};Database={Db};Timeout=5;SslMode=Disable;Trust Server Certificate=true;";
    }
}
