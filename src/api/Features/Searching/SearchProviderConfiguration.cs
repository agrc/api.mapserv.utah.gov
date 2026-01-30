namespace ugrc.api.Features.Searching;

public class SearchProviderConfiguration {
    public string Host { get; set; } = "opensgid.ugrc.utah.gov";
    public string Db { get; set; } = "opensgid";
    public string Username { get; set; } = "agrc";
    public string Password { get; set; } = "agrc";

    public string ConnectionString =>
        $"Host={Host};Username={Username};Password={Password};Database={Db};Timeout=5;SslMode=Disable;";
}
