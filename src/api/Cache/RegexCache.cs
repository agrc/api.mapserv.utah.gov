namespace ugrc.api.Cache;
public partial class RegexCache : IRegexCache {
    private readonly IAbbreviations _abbreviations;
    private readonly Dictionary<string, Regex> _regexDictionary;

    public RegexCache(IAbbreviations abbreviations) {
        _abbreviations = abbreviations;
        _regexDictionary = new Dictionary<string, Regex> {
            {
                "direction", direction()
            }, {
                "directionSubstitutions", directionSubstitutions()
            }, {
                "streetType", BuildStreetTypeRegularExpression()
            }, {
                "unitType", new Regex(BuildUnitTypeRegularExpression(), RegexOptions.IgnoreCase | RegexOptions.Compiled)
            }, {
                "unitTypeLookBehind",
                new Regex(BuildUnitTypeRegularExpression() + @"(?:\s?#)",
                          RegexOptions.IgnoreCase | RegexOptions.Compiled)
            }, {
                "highway", highway()
            }, {
                "separateNameAndDirection", separateNameAndDirection()
            }, {
                "streetNumbers", streetNumbers()
            }, {
                "ordinal", ordinal()
            }, {
                "pobox", pobox()
            }, {
                "zipPlusFour", zipPlusFour()
            }, {
                "cityName", cityName()
            }, {
                "cityTownCruft", cityTownCruft()
            }, {
                "stripCitySuffix", stripCitySuffix()
            }, {
                "stripStateSuffix", stripStateSuffix()
            }, {
                "avesOrdinal", BuildOrdinalRegex()
            }
        };
    }

    public Regex Get(string key) => _regexDictionary[key];

    private string BuildUnitTypeRegularExpression() {
        var pattern = new List<string>();

        foreach (var item in _abbreviations.UnitAbbreviations) {
            var abbreviations = new List<string> {
                item.Item1,
                item.Item2
            };

            pattern.Add(string.Format(@"\b({0})\b", string.Join("|", abbreviations)));
        }

        return string.Format("({0})", string.Join("|", pattern));
    }

    private Regex BuildStreetTypeRegularExpression() {
        var pattern = new List<string>();

        if (_abbreviations.StreetTypeAbbreviations != null) {
            foreach (var streetType in _abbreviations.StreetTypeAbbreviations) {
                var abbreviations = streetType.Value.Split(',').ToList();
                abbreviations.Add(streetType.Key.ToString());

                pattern.Add(string.Format(@"\b({0})\b", string.Join("|", abbreviations)));
            }
        }

        return new Regex(string.Format("({0})", string.Join("|", pattern)),
                         RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private Regex BuildOrdinalRegex() {
        // avenues in slc go to 18. 1-8 in midvale
        var ordinals = Enumerable.Range(1, 18)
                                 .Select(ToOrdinal)
                                 .Concat(Enumerable.Range(1, 18).Select(x => x.ToString()))
                                 .Concat([
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
                                 ]);

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

        return (number % 10) switch {
            1 => number + "st",
            2 => number + "nd",
            3 => number + "rd",
            _ => number + "th",
        };
    }

    [GeneratedRegex("\\b(north|n\\.?)(?!\\w)|\\b(south|s\\.?)(?!\\w)|\\b(east|e\\.?)(?!\\w)|\\b(west|w\\.?)(?!\\w)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex direction();
    [GeneratedRegex("\\b((so|sou|sth)|(no|nor|nrt)|(ea|eas|est)|(we|wst|wes))(?=\\s|\\.|$)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex directionSubstitutions();
    [GeneratedRegex("\\b(sr|state route|us|Highway|hwy|u.s\\.?)(?!\\w)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex highway();
    [GeneratedRegex("\\b(\\d+)(s|south|e|east|n|north|w|west)\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex separateNameAndDirection();
    [GeneratedRegex("\\b(?<!-#)(\\d+)", RegexOptions.Compiled)]
    private static partial Regex streetNumbers();
    [GeneratedRegex("^(\\d+)(?:st|nd|rd|th)\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex ordinal();
    [GeneratedRegex("p\\s*o\\s*box\\s*(\\d+)\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex pobox();
    [GeneratedRegex("(^\\d{5})-?(\\d{4})?$", RegexOptions.Compiled)]
    private static partial Regex zipPlusFour();
    [GeneratedRegex("^[ a-z\\.]+", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex cityName();
    [GeneratedRegex("(?:city|town)(?: of)", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex cityTownCruft();
    [GeneratedRegex("\\b(?:cty|city)$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex stripCitySuffix();
    [GeneratedRegex("\\b(?:utah|ut)$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex stripStateSuffix();
}
