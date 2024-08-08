namespace ugrc.api.Models.Configuration;
public record DatabaseConfiguration {
    public string ConnectionString => $"{Host}:{Port},connectTimeout=200,connectRetry=1,abortConnect=false";

    public string? Host { get; set; }
    public int Port { get; set; } = 6379;
}
