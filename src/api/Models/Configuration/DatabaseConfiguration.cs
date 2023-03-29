#nullable enable
namespace AGRC.api.Models.Configuration;
// TODO Duplicated in developer.mapserv.utah.gov
public record DatabaseConfiguration(string Host, string Port) {
    public string ConnectionString => $"{Host}:{Port},abortConnect=false";
}
