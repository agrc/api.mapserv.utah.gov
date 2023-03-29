using AGRC.api.Models.Constants;

namespace AGRC.api.Cache;
public interface IAbbreviations {
    Dictionary<StreetType, string> StreetTypeAbbreviations { get; }
    List<Tuple<string, string, bool>> UnitAbbreviations { get; }
}
