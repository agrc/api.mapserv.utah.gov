namespace api.mapserv.utah.gov.Models.Configuration {
    // TODO Duplicated in developer.mapserv.utah.gov
    public class DatabaseConfiguration {
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "5432";
        public string Db { get; set; } = "webapi";
        public string Username { get; set; } = "postgres";
        public string Password { get; set; } = "what password";

        public string ConnectionString =>
            $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Db}";
    }
}
