using System;
namespace developer.mapserv.utah.gov.Models.Configuration
{
    public class DatabaseConfiguration
    {
        public string Host { get; set; } = "localhost";
        public string Port { get; set; } = "5432";
        public string Db { get; set; } = "webapi";
        public string Username { get; set; } = "postgres";
        public string Password { get; set; } = "what password";

        public string ConnectionString
        {
            get => $"Host={Host};Port={Port};Username={Username};Password={Password};Database={Db}";
        }
    }
}
