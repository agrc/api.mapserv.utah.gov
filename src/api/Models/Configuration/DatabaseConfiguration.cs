namespace AGRC.api.Models.Configuration;
// TODO Duplicated in developer.mapserv.utah.gov
public record DatabaseConfiguration {
    public string ConnectionString => $"{Host}:{Port},abortConnect=false";

    public string? Host { get; set; }
    public int Port { get; set; } = 6379;
}
