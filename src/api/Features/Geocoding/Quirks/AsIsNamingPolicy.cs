using System.Text.Json;

#nullable enable
namespace AGRC.api.Geocoding;
public class AsIsNamingPolicy : JsonNamingPolicy {
    public override string ConvertName(string name) => name;
}
