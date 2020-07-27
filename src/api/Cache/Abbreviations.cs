using System;
using System.Collections.Generic;
using System.Globalization;
using AGRC.api.Models.Constants;

namespace AGRC.api.Cache {
    public class Abbreviations : IAbbreviations {
        public Dictionary<StreetType, string> StreetTypeAbbreviations => new Dictionary<StreetType, string> {
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

        public List<Tuple<string, string, bool>> UnitAbbreviations => new List<Tuple<string, string, bool>> {
            new Tuple<string, string, bool>(SecondaryUnit.Apartment.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "apt", true),
            new Tuple<string, string, bool>(SecondaryUnit.Basement.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "bsmt", false),
            new Tuple<string, string, bool>(SecondaryUnit.Building.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "bldg", true),
            new Tuple<string, string, bool>(SecondaryUnit.Department.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "dept", true),
            new Tuple<string, string, bool>(SecondaryUnit.Floor.ToString().ToLower(CultureInfo.InvariantCulture), "fl",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Front.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "frnt", false),
            new Tuple<string, string, bool>(SecondaryUnit.Hanger.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "hngr", true),
            new Tuple<string, string, bool>(SecondaryUnit.Key.ToString().ToLower(CultureInfo.InvariantCulture), "key",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Lobby.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "lbby", false),
            new Tuple<string, string, bool>(SecondaryUnit.Lot.ToString().ToLower(CultureInfo.InvariantCulture), "lot",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Lower.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "lowr", false),
            new Tuple<string, string, bool>(SecondaryUnit.Office.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "ofc", false),
            new Tuple<string, string, bool>(SecondaryUnit.Penthouse.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "ph", false),
            new Tuple<string, string, bool>(SecondaryUnit.Pier.ToString().ToLower(CultureInfo.InvariantCulture), "pier",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Rear.ToString().ToLower(CultureInfo.InvariantCulture), "rear",
                                            false),
            new Tuple<string, string, bool>(SecondaryUnit.Room.ToString().ToLower(CultureInfo.InvariantCulture), "rm",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Side.ToString().ToLower(CultureInfo.InvariantCulture), "side",
                                            false),
            new Tuple<string, string, bool>(SecondaryUnit.Slip.ToString().ToLower(CultureInfo.InvariantCulture), "slip",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Space.ToString().ToLower(CultureInfo.InvariantCulture), "spc",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Stop.ToString().ToLower(CultureInfo.InvariantCulture), "stop",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Suite.ToString().ToLower(CultureInfo.InvariantCulture), "ste",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Trailer.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "trlr", true),
            new Tuple<string, string, bool>(SecondaryUnit.Unit.ToString().ToLower(CultureInfo.InvariantCulture), "unit",
                                            true),
            new Tuple<string, string, bool>(SecondaryUnit.Upper.ToString().ToLower(CultureInfo.InvariantCulture),
                                            "uppr", false)
        };
    }
}
