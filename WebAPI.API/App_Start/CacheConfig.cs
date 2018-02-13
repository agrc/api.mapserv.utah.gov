using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using WebAPI.API.Commands.Address;
using WebAPI.API.Commands.Drive;
using WebAPI.API.Commands.Sgid;
using WebAPI.Common.Executors;
using WebAPI.Domain;

namespace WebAPI.API
{
    public static class CacheConfig
    {
        public static void BuildCache()
        {
            var httpClientHandler = new HttpClientHandler();
            if (httpClientHandler.SupportsAutomaticDecompression)
            {
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            App.HttpClient = new HttpClient(httpClientHandler)
            {
                Timeout = new TimeSpan(0, 0, 60)
            };

            App.GridsWithAddressPoints = CommandExecutor.ExecuteCommand(new GetAddressGridsWithAddressPointsCommand())
                                         ?? new List<string>();
            App.StreetTypeAbbreviations = CacheStreetTypeAbbreviations();
            App.UnitAbbreviations = CacheUnitAbbreviations();
            App.SgidCategories = CommandExecutor.ExecuteCommand(new GetSgidCategoriesCommand()) ?? new List<string>();

            var lookups = CommandExecutor.ExecuteCommand(new GetCachedDataFromDriveCommand());
            App.PlaceGridLookup = lookups.PlaceGrids;
            App.ZipCodeGridLookup = lookups.ZipCodesGrids;
            App.UspsDeliveryPoints = lookups.UspsDeliveryPoints;

            var exclusions = lookups.PoBoxExclusions.ToList();
            App.PoBoxZipCodesWithExclusions = exclusions.Select(x => x.Zip).Distinct();
            App.PoBoxExclusions = exclusions.ToDictionary(x => x.ZipPlusFour, y => y);

            App.PoBoxLookup = CommandExecutor.ExecuteCommand(new GetPoBoxLocationsCommand());

            App.RegularExpressions = CacheRegularExpressions();
        }

        /// <summary>
        ///     Gets the abbreviations.
        ///     Types with no abbreviations are omitted.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<StreetType, string> CacheStreetTypeAbbreviations()
        {
            return new Dictionary<StreetType, string>
            {
                {
                    StreetType.Alley, "aly,alee"
                },
                {
                    StreetType.Avenue, "av,ave"
                },
                {
                    StreetType.Boulevard, "blvd"
                },
                {
                    StreetType.Canyon, "canyn,cyn,cnyn"
                },
                {
                    StreetType.Circle, "cir"
                },
                {
                    StreetType.Center, "ctr,cnter,cntr,cntre"
                },
                {
                    StreetType.Corner, "cor"
                },
                {
                    StreetType.Court, "ct"
                },
                {
                    StreetType.Creek, "ck,crk,cr"
                },
                {
                    StreetType.Crossing, "xing"
                },
                {
                    StreetType.Cove, "cv"
                },
                {
                    StreetType.Drive, "dr"
                },
                {
                    StreetType.Estate, "est"
                },
                {
                    StreetType.Estates, "ests"
                },
                {
                    StreetType.Expressway, "expy"
                },
                {
                    StreetType.Fork, "frk"
                },
                {
                    StreetType.Freeway, "fwy"
                },
                {
                    StreetType.Heights, "hts"
                },
                {
                    StreetType.Highway, "hwy"
                },
                {
                    StreetType.Hill, "hl"
                },
                {
                    StreetType.Hollow, "holw,hllw"
                },
                {
                    StreetType.Junction, "jct"
                },
                {
                    StreetType.Lane, "ln"
                },
                {
                    StreetType.Loop, "loop"
                },
                {
                    StreetType.Parkway, "pkwy"
                },
                {
                    StreetType.Place, "pl"
                },
                {
                    StreetType.Plaza, "plz"
                },
                {
                    StreetType.Point, "pt"
                },
                {
                    StreetType.Ranch, "rnch"
                },
                {
                    StreetType.Ridge, "rdg"
                },
                {
                    StreetType.Road, "rd"
                },
                {
                    StreetType.Route, "rte"
                },
                {
                    StreetType.Square, "sq,sqr"
                },
                {
                    StreetType.Street, "st,str"
                },
                {
                    StreetType.Terrace, "ter"
                },
                {
                    StreetType.Trail, "trl"
                },
                {
                    StreetType.Village, "vlg,villag,villg"
                },
                {
                    StreetType.View, "vw"
                },
                {
                    StreetType.Way, "wy"
                }
            };
        }

        private static List<Tuple<string, string, bool>> CacheUnitAbbreviations()
        {
            //Number nbr num
            return new List<Tuple<string, string, bool>>
            {
                new Tuple<string, string, bool>(SecondaryUnit.Apartment.ToString().ToLower(CultureInfo.InvariantCulture), "apt", true),
                new Tuple<string, string, bool>(SecondaryUnit.Basement.ToString().ToLower(CultureInfo.InvariantCulture), "bsmt", false),
                new Tuple<string, string, bool>(SecondaryUnit.Building.ToString().ToLower(CultureInfo.InvariantCulture), "bldg", true),
                new Tuple<string, string, bool>(SecondaryUnit.Department.ToString().ToLower(CultureInfo.InvariantCulture), "dept", true),
                new Tuple<string, string, bool>(SecondaryUnit.Floor.ToString().ToLower(CultureInfo.InvariantCulture), "fl", true),
                new Tuple<string, string, bool>(SecondaryUnit.Front.ToString().ToLower(CultureInfo.InvariantCulture), "frnt", false),
                new Tuple<string, string, bool>(SecondaryUnit.Hanger.ToString().ToLower(CultureInfo.InvariantCulture), "hngr", true),
                new Tuple<string, string, bool>(SecondaryUnit.Key.ToString().ToLower(CultureInfo.InvariantCulture), "key", true),
                new Tuple<string, string, bool>(SecondaryUnit.Lobby.ToString().ToLower(CultureInfo.InvariantCulture), "lbby", false),
                new Tuple<string, string, bool>(SecondaryUnit.Lot.ToString().ToLower(CultureInfo.InvariantCulture), "lot", true),
                new Tuple<string, string, bool>(SecondaryUnit.Lower.ToString().ToLower(CultureInfo.InvariantCulture), "lowr", false),
                new Tuple<string, string, bool>(SecondaryUnit.Office.ToString().ToLower(CultureInfo.InvariantCulture), "ofc", false),
                new Tuple<string, string, bool>(SecondaryUnit.Penthouse.ToString().ToLower(CultureInfo.InvariantCulture), "ph", false),
                new Tuple<string, string, bool>(SecondaryUnit.Pier.ToString().ToLower(CultureInfo.InvariantCulture), "pier", true),
                new Tuple<string, string, bool>(SecondaryUnit.Rear.ToString().ToLower(CultureInfo.InvariantCulture), "rear", false),
                new Tuple<string, string, bool>(SecondaryUnit.Room.ToString().ToLower(CultureInfo.InvariantCulture), "rm", true),
                new Tuple<string, string, bool>(SecondaryUnit.Side.ToString().ToLower(CultureInfo.InvariantCulture), "side", false),
                new Tuple<string, string, bool>(SecondaryUnit.Slip.ToString().ToLower(CultureInfo.InvariantCulture), "slip", true),
                new Tuple<string, string, bool>(SecondaryUnit.Space.ToString().ToLower(CultureInfo.InvariantCulture), "spc", true),
                new Tuple<string, string, bool>(SecondaryUnit.Stop.ToString().ToLower(CultureInfo.InvariantCulture), "stop", true),
                new Tuple<string, string, bool>(SecondaryUnit.Suite.ToString().ToLower(CultureInfo.InvariantCulture), "ste", true),
                new Tuple<string, string, bool>(SecondaryUnit.Trailer.ToString().ToLower(CultureInfo.InvariantCulture), "trlr", true),
                new Tuple<string, string, bool>(SecondaryUnit.Unit.ToString().ToLower(CultureInfo.InvariantCulture), "unit", true),
                new Tuple<string, string, bool>(SecondaryUnit.Upper.ToString().ToLower(CultureInfo.InvariantCulture), "uppr", false)
            };
        }

        private static Dictionary<string, Regex> CacheRegularExpressions()
        {
            var dict = new Dictionary<string, Regex>
            {
                {
                    "direction",
                    new Regex(@"\b(north|n\.?)(?!\w)|\b(south|s\.?)(?!\w)|\b(east|e\.?)(?!\w)|\b(west|w\.?)(?!\w)",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "directionSubstitutions",
                    new Regex(@"\b((so|sou|sth)|(no|nor|nrt)|(ea|eas|est)|(we|wst|wes))(?=\s|\.|$)",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "streetType",
                    BuildStreetTypeRegularExpression()
                },
                {
                    "unitType",
                    new Regex(BuildUnitTypeRegularExpression(), RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "unitTypeLookBehind",
                    new Regex(BuildUnitTypeRegularExpression() + @"(?:\s?#)",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "highway",
                    new Regex(@"\b(sr|state route|us|Highway|hwy|u.s\.?)(?!\w)",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "separateNameAndDirection",
                    new Regex(@"\b(\d+)(s|south|e|east|n|north|w|west)\b",
                        RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "streetNumbers",
                    new Regex(@"\b(?<!-#)(\d+)", RegexOptions.Compiled)
                },
                {
                    "ordinal",
                    new Regex(@"^(\d+)(?:st|nd|rd|th)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                },
                {
                    "pobox",
                    new Regex(@"p\s*o\s*box\s*(\d+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                },
                {
                    "zipPlusFour",
                    new Regex(@"(^\d{5})-?(\d{4})?$", RegexOptions.Compiled)
                },
                {
                    "cityName",
                    new Regex(@"^[ a-z\.]+", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                },
                {
                    "cityTownCruft",
                    new Regex(@"(?:city|town)(?: of)", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                }
            };

            return dict;
        }

        private static string BuildUnitTypeRegularExpression()
        {
            var pattern = new List<string>();

            foreach (var item in App.UnitAbbreviations)
            {
                var abbrs = new List<string>
                {
                    item.Item1,
                    item.Item2
                };

                pattern.Add(string.Format(@"\b({0})\b", string.Join("|", abbrs)));
            }

            return string.Format("({0})", string.Join("|", pattern));
        }

        private static Regex BuildStreetTypeRegularExpression()
        {
            var pattern = new List<string>();

            if (App.StreetTypeAbbreviations != null)
            {
                foreach (var streetType in App.StreetTypeAbbreviations)
                {
                    var abbrs = streetType.Value.Split(',').ToList();
                    abbrs.Add(streetType.Key.ToString());

                    pattern.Add(string.Format(@"\b({0})\b", string.Join("|", abbrs)));
                }
            }

            return new Regex(string.Format("({0})", string.Join("|", pattern)),
                RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
    }
}
