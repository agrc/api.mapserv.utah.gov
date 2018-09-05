using System;
using System.Collections.Generic;
using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Cache {
    public interface IAbbreviations {
        Dictionary<StreetType, string> StreetTypeAbbreviations { get; }
        List<Tuple<string, string, bool>> UnitAbbreviations { get; }
    }
}
