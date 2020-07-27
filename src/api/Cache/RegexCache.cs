using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AGRC.api.Cache {
    public class RegexCache : IRegexCache {
        private readonly IAbbreviations _abbreviations;
        private readonly Dictionary<string, Regex> _regexDictionary;

        public RegexCache(IAbbreviations abbreviations) {
            _abbreviations = abbreviations;
            _regexDictionary = new Dictionary<string, Regex> {
                {
                    "direction",
                    new Regex(@"\b(north|n\.?)(?!\w)|\b(south|s\.?)(?!\w)|\b(east|e\.?)(?!\w)|\b(west|w\.?)(?!\w)",
                              RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "directionSubstitutions",
                    new Regex(@"\b((so|sou|sth)|(no|nor|nrt)|(ea|eas|est)|(we|wst|wes))(?=\s|\.|$)",
                              RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "streetType", BuildStreetTypeRegularExpression()
                }, {
                    "unitType",
                    new Regex(BuildUnitTypeRegularExpression(), RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "unitTypeLookBehind",
                    new Regex(BuildUnitTypeRegularExpression() + @"(?:\s?#)",
                              RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "highway",
                    new Regex(@"\b(sr|state route|us|Highway|hwy|u.s\.?)(?!\w)",
                              RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "separateNameAndDirection",
                    new Regex(@"\b(\d+)(s|south|e|east|n|north|w|west)\b",
                              RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "streetNumbers",
                    new Regex(@"\b(?<!-#)(\d+)", RegexOptions.Compiled)
                }, {
                    "ordinal",
                    new Regex(@"^(\d+)(?:st|nd|rd|th)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled)
                }, {
                    "pobox",
                    new Regex(@"p\s*o\s*box\s*(\d+)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                }, {
                    "zipPlusFour",
                    new Regex(@"(^\d{5})-?(\d{4})?$", RegexOptions.Compiled)
                }, {
                    "cityName",
                    new Regex(@"^[ a-z\.]+", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                }, {
                    "cityTownCruft",
                    new Regex(@"(?:city|town)(?: of)", RegexOptions.Compiled | RegexOptions.IgnoreCase)
                }, {
                    "avesOrdinal",
                    BuildOridnalRegex()
                }
            };
        }

        public Regex Get(string key) => _regexDictionary[key];

        private string BuildUnitTypeRegularExpression() {
            var pattern = new List<string>();

            foreach (var item in _abbreviations.UnitAbbreviations) {
                var abbrs = new List<string> {
                    item.Item1,
                    item.Item2
                };

                pattern.Add(string.Format(@"\b({0})\b", string.Join("|", abbrs)));
            }

            return string.Format("({0})", string.Join("|", pattern));
        }

        private Regex BuildStreetTypeRegularExpression() {
            var pattern = new List<string>();

            if (_abbreviations.StreetTypeAbbreviations != null) {
                foreach (var streetType in _abbreviations.StreetTypeAbbreviations) {
                    var abbrs = streetType.Value.Split(',').ToList();
                    abbrs.Add(streetType.Key.ToString());

                    pattern.Add(string.Format(@"\b({0})\b", string.Join("|", abbrs)));
                }
            }

            return new Regex(string.Format("({0})", string.Join("|", pattern)),
                             RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        private Regex BuildOridnalRegex() {
            // avenues in slc go to 18. 1-8 in midvale
            var ordinals = Enumerable.Range(1, 18)
                                     .Select(ToOrdinal)
                                     .Concat(Enumerable.Range(1, 18).Select(x => x.ToString()))
                                     .Concat(new[] {
                                         "one", "first",
                                         "two", "second",
                                         "three", "third",
                                         "four", "fourth",
                                         "five", "fifth",
                                         "six", "sixth",
                                         "seven", "seventh",
                                         "eight", "eighth",
                                         "nine", "ninth",
                                         "ten", "tenth",
                                         "eleven", "eleventh",
                                         "twelve", "twelfth",
                                         "thirteen", "thirteenth",
                                         "fourteen", "fourteenth",
                                         "fifteen", "fifteenth",
                                         "sixteen", "sixteenth",
                                         "seventeen", "seventeenth",
                                         "eighteen", "eighteenth"
                                     });

            return new Regex(string.Format("^({0})$", string.Join("|", ordinals)), RegexOptions.IgnoreCase);
        }

        private static string ToOrdinal(int number) {
            if (number < 0) {
                return number.ToString();
            }

            var rem = number % 100;
            if (rem >= 11 && rem <= 13) {
                return number + "th";
            }

            return (number % 10) switch
            {
                1 => number + "st",
                2 => number + "nd",
                3 => number + "rd",
                _ => number + "th",
            };
        }
    }
}
