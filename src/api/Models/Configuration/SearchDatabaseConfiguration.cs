namespace AGRC.api.Models.Configuration {
    // TODO Duplicated in developer.mapserv.utah.gov
    public class SearchDatabaseConfiguration {
        public string Host { get; set; } = "sgid.agrc.utah.gov";
        public string Db { get; set; } = "sgid10";
        public string Username { get; set; } = "agrc";
        public string Password { get; set; } = "agrc";

        public string ConnectionString =>
            $"Server={Host};User Id={Username};Password={Password};Database={Db}";
    }
}
