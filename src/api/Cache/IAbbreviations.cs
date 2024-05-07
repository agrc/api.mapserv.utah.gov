using ugrc.api.Models.Constants;

namespace ugrc.api.Cache;
public interface IAbbreviations {
    Dictionary<StreetType, string> StreetTypeAbbreviations { get; }
    List<Tuple<string, string, bool>> UnitAbbreviations { get; }
}
