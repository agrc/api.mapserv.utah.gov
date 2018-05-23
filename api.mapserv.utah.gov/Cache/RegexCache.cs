using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace api.mapserv.utah.gov.Cache
{
    public class RegexCache : IRegexCache
    {
        private readonly IAbbreviations _abbreviations;
        private readonly Dictionary<string, Regex> _regexDictionary;

        public RegexCache(IAbbreviations abbreviations)
        {
            _abbreviations = abbreviations;
            _regexDictionary = new Dictionary<string, Regex>
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
                    "streetType", BuildStreetTypeRegularExpression()
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
        }

        public Regex Get(string key)
        {
            return _regexDictionary[key];
        }

        private string BuildUnitTypeRegularExpression()
        {
            var pattern = new List<string>();

            foreach (var item in _abbreviations.UnitAbbreviations)
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

        private Regex BuildStreetTypeRegularExpression()
        {
            var pattern = new List<string>();

            if (_abbreviations.StreetTypeAbbreviations != null)
            {
                foreach (var streetType in _abbreviations.StreetTypeAbbreviations)
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
