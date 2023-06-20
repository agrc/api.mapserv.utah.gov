namespace AGRC.api.Geocoding;
public class AsIsNamingPolicy : JsonNamingPolicy {
    public override string ConvertName(string name) => name;
}
