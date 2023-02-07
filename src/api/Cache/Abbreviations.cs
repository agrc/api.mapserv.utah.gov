using System;
using System.Collections.Generic;
using System.Globalization;
using AGRC.api.Models.Constants;

namespace AGRC.api.Cache {
    public class Abbreviations : IAbbreviations {
        public Dictionary<StreetType, string> StreetTypeAbbreviations => new() {
            {
                StreetType.Alley, "aly,alee"
            }, {
                StreetType.Avenue, "av,ave"
            }, {
                StreetType.Boulevard, "blvd"
            }, {
                StreetType.Canyon, "canyn,cyn,cnyn"
            }, {
                StreetType.Circle, "cir"
            }, {
                StreetType.Center, "ctr,cnter,cntr,cntre"
            }, {
                StreetType.Corner, "cor"
            }, {
                StreetType.Court, "ct"
            }, {
                StreetType.Creek, "ck,crk,cr"
            }, {
                StreetType.Crossing, "xing"
            }, {
                StreetType.Cove, "cv"
            }, {
                StreetType.Drive, "dr"
            }, {
                StreetType.Estate, "est"
            }, {
                StreetType.Estates, "ests"
            }, {
                StreetType.Expressway, "expy"
            }, {
                StreetType.Fork, "frk"
            }, {
                StreetType.Freeway, "fwy"
            }, {
                StreetType.Heights, "hts"
            }, {
                StreetType.Highway, "hwy"
            }, {
                StreetType.Hill, "hl"
            }, {
                StreetType.Hollow, "holw,hllw"
            }, {
                StreetType.Junction, "jct"
            }, {
                StreetType.Lane, "ln"
            }, {
                StreetType.Loop, "loop"
            }, {
                StreetType.Parkway, "pkwy"
            }, {
                StreetType.Place, "pl"
            }, {
                StreetType.Plaza, "plz"
            }, {
                StreetType.Point, "pt"
            }, {
                StreetType.Ranch, "rnch"
            }, {
                StreetType.Ridge, "rdg"
            }, {
                StreetType.Road, "rd"
            }, {
                StreetType.Route, "rte"
            }, {
                StreetType.Square, "sq,sqr"
            }, {
                StreetType.Street, "st,str"
            }, {
                StreetType.Terrace, "ter"
            }, {
                StreetType.Trail, "trl"
            }, {
                StreetType.Village, "vlg,villag,villg"
            }, {
                StreetType.View, "vw"
            }, {
                StreetType.Way, "wy"
            }
        };

        public List<Tuple<string, string, bool>> UnitAbbreviations => new() {
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Apartment).ToLower(CultureInfo.InvariantCulture),
                                            "apt", true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Basement).ToLower(CultureInfo.InvariantCulture),
                                            "bsmt", false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Building).ToLower(CultureInfo.InvariantCulture),
                                            "bldg", true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Department).ToLower(CultureInfo.InvariantCulture),
                                            "dept", true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Floor).ToLower(CultureInfo.InvariantCulture), "fl",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Front).ToLower(CultureInfo.InvariantCulture),
                                            "frnt", false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Hanger).ToLower(CultureInfo.InvariantCulture),
                                            "hngr", true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Key).ToLower(CultureInfo.InvariantCulture), "key",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Lobby).ToLower(CultureInfo.InvariantCulture),
                                            "lbby", false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Lot).ToLower(CultureInfo.InvariantCulture), "lot",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Lower).ToLower(CultureInfo.InvariantCulture),
                                            "lowr", false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Office).ToLower(CultureInfo.InvariantCulture),
                                            "ofc", false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Penthouse).ToLower(CultureInfo.InvariantCulture),
                                            "ph", false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Pier).ToLower(CultureInfo.InvariantCulture), "pier",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Rear).ToLower(CultureInfo.InvariantCulture), "rear",
                                            false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Room).ToLower(CultureInfo.InvariantCulture), "rm",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Side).ToLower(CultureInfo.InvariantCulture), "side",
                                            false),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Slip).ToLower(CultureInfo.InvariantCulture), "slip",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Space).ToLower(CultureInfo.InvariantCulture), "spc",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Stop).ToLower(CultureInfo.InvariantCulture), "stop",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Suite).ToLower(CultureInfo.InvariantCulture), "ste",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Trailer).ToLower(CultureInfo.InvariantCulture),
                                            "trlr", true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Unit).ToLower(CultureInfo.InvariantCulture), "unit",
                                            true),
            new Tuple<string, string, bool>(nameof(SecondaryUnit.Upper).ToLower(CultureInfo.InvariantCulture),
                                            "uppr", false)
        };
    }
}
