using AGRC.api.Cache;
using AGRC.api.Comparers;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;

namespace AGRC.api.Features.Geocoding;
public partial class AddressParsing {
    public class Computation : IComputation<Address> {
        public Computation(string street) {
            Street = street;
        }

        internal string Street { get; set; }
    }

    public partial class Handler : IComputationHandler<Computation, Address> {
        private readonly IAbbreviations _abbreviations;
        private readonly ILogger? _log;
        private readonly IRegexCache _regexCache;
        private string Street { get; set; }
        private string OriginalStreet { get; set; }
        private string StandardStreet { get; set; }

        public Handler(IRegexCache regexCache, IAbbreviations abbreviations, ILogger? log) {
            _regexCache = regexCache;
            _abbreviations = abbreviations;
            _log = log?.ForContext<AddressParsing>();

            Street = string.Empty;
            OriginalStreet = string.Empty;
            StandardStreet = string.Empty;
        }

        public Task<Address> Handle(Computation request, CancellationToken cancellationToken) {
            OriginalStreet = request.Street;
            Street = request.Street;

            Street = ReplaceExtraCharacters(Street);
            Street = ReplaceHighway(Street, _regexCache.Get("highway"), out var isHighway);
            Street = ReplaceUnitTypes(Street, _regexCache.Get("unitType"), _regexCache.Get("unitTypeLookBehind"), _abbreviations.UnitAbbreviations);
            Street = ReplaceDirections(Street, _regexCache.Get("directionSubstitutions"));
            Street = SplitNumbersAndLetters(Street, _regexCache.Get("separateNameAndDirection"));
            StandardStreet = Street;

            if (TryParsePoBox(Street, _regexCache.Get("pobox"), out var address)) {
                return Task.FromResult(address);
            }

            Street = ParseNumbers(Street, isHighway,
                _regexCache.Get("streetNumbers"),
                _regexCache.Get("ordinal"),
                _regexCache.Get("direction"),
                out var houseNumber, out var streetName
            );
            Street = ParseStreetType(Street, isHighway, _regexCache.Get("streetType"), _abbreviations.StreetTypeAbbreviations, out var streetType);
            Street = ParseDirections(Street, streetName, streetType, StandardStreet, _regexCache.Get("direction"), _abbreviations.StreetTypeAbbreviations, out var prefixDirection, out var suffixDirection);
            streetName = ParseStreetName(Street, streetName);

            _log?.ForContext("original", OriginalStreet)
                .ForContext("standardized", StandardStreet)
                .Information("address parsing complete");

            return Task.FromResult(new Address(OriginalStreet,
                houseNumber, prefixDirection, streetName, streetType, suffixDirection,
                0, 0, null, 0, false, isHighway));
        }

        private static string ReplaceExtraCharacters(string street) {
            street = street.Replace(".", string.Empty).Replace(",", string.Empty).Replace("_", " ");

            return MultipleSpaces().Replace(street, " ").Trim();
        }
        //make space between 500west = 500 west
        private static string SplitNumbersAndLetters(string street, Regex numbersAndLetters) =>
            numbersAndLetters.Replace(street, "$1 $2");
        private static string ReplaceHighway(string street, Regex highwayRegex, out bool isHighway) {
            var original = street;
            street = highwayRegex.Replace(original, "Highway");
            isHighway = street != original;

            return street.Trim();
        }
        private static string ReplaceUnitTypes(string street, Regex unitTypeRegex, Regex unitTypeBehindRegex, List<Tuple<string, string, bool>> unitAbbreviations) {
            var matches = unitTypeRegex.Matches(street);

            //probably a secondary address
            if (street.Contains('#')) {
                var indexOfValue = "#";

                //check to see if the # is preceded by a secondary address unit type
                var unitMatches = unitTypeBehindRegex.Matches(street);
                if (unitMatches.Count > 0) {
                    indexOfValue = unitMatches[0].Value;
                }

                var index = street.LastIndexOf(indexOfValue, StringComparison.OrdinalIgnoreCase);
                street = street[..index].Trim();
            }

            if (matches.Count == 0) {
                return street;
            }

            //get last match since street name could be in there?
            var match = matches[^1].Value.ToLower();

            var unitType = unitAbbreviations.Single(x => x.Item1 == match || x.Item2 == match);

            if (!unitType.Item3) {
                // unit doesn't need a number after it. check that it is at
                // the end of the street and remove it if it is.

                if (!street.EndsWith(match, StringComparison.OrdinalIgnoreCase)) {
                    return street;
                }

                var index = street.LastIndexOf(match, StringComparison.OrdinalIgnoreCase);

                return string.Concat(street.AsSpan(0, index), street.AsSpan(index + match.Length)).Trim();
            }

            //make sure address has a number after it since it's required.
            var regex = new Regex(string.Format(@"{0}(?:\s|.)?([a-z]?(?:\d)*[a-z]?)", match),
                                  RegexOptions.IgnoreCase);
            var moreMatches = regex.Matches(street);

            if (moreMatches.Count > 0 &&
                street.EndsWith(moreMatches[^1].Value, StringComparison.OrdinalIgnoreCase)) {
                var theMatch = moreMatches[^1];
                var index = street.LastIndexOf(theMatch.Value, StringComparison.OrdinalIgnoreCase);

                return string.Concat(street.AsSpan(0, index), street.AsSpan(index + theMatch.Length)).Trim();
            }

            return street.Trim();
        }
        private static string ReplaceDirections(string street, Regex directionSubstitutionsRegex) {
            var match = directionSubstitutionsRegex.Match(street);
            if (!match.Success) {
                return street;
            }

            static string Replace(string original, string matchedValue, string replaced) => original.Replace(matchedValue, replaced);

            while (match.Success) {
                var value = match.Value;
                switch (value.ToLowerInvariant()[0]) {
                    case 'n':
                        street = Replace(street, value, "north");
                        break;
                    case 's':
                        street = Replace(street, value, "south");
                        break;
                    case 'e':
                        street = Replace(street, value, "east");
                        break;
                    case 'w':
                        street = Replace(street, value, "west");
                        break;
                }

                match = match.NextMatch();
            }

            return street.Trim();
        }
        private static bool TryParsePoBox(string street, Regex poboxRegex, out Address poboxAddress) {
            var match = poboxRegex.Match(street);
            if (!match.Success) {
                poboxAddress = null!;

                return false;
            }

            var pobox = match.Groups[1].Value;
            if (!int.TryParse(pobox, out var poboxValue)) {
                poboxAddress = null!;

                return false;
            }

            poboxAddress = Address.BuildPoBoxAddress(street, poboxValue, 0);

            return true;
        }
        public static bool TryParseDirection(string part, out Direction direction) {
            direction = Direction.None;

            if (string.IsNullOrEmpty(part)) {
                return false;
            }

            if (part.Length > 1) {
                if (Enum.TryParse(part, true, out direction)) {
                    return true;
                }
            } else {
                switch (part.ToLower()) {
                    case "n":
                        direction = Direction.North;
                        return true;
                    case "s":
                        direction = Direction.South;
                        return true;
                    case "e":
                        direction = Direction.East;
                        return true;
                    case "w":
                        direction = Direction.West;
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }
        private static string ParseNumbers(string street, bool isHighway, Regex streetNumbers, Regex ordinalRegex, Regex directionRegex, out int? houseNumber, out string streetName) {
            var matches = streetNumbers.Matches(street);
            houseNumber = null;
            streetName = string.Empty;
            int number;

            switch (matches.Count) {
                case 1: { // 123 south main street
                        street = street.Remove(street.IndexOf(matches[0].Value, StringComparison.OrdinalIgnoreCase),
                                               matches[0].Length).Trim();
                        if (int.TryParse(matches[0].Value, out number)) {
                            houseNumber = number;
                        }

                        break;
                    }
                case 2: // 2236 east 300 south or 401 N highway 68
                case 3: {
                        if (isHighway) {
                            var parts = street.Split(' ').ToList();
                            var numberIndex = parts.IndexOf(matches[1].Value);

                            var highwayIndex = numberIndex - 1;

                            if (highwayIndex > -1 && parts[highwayIndex].Contains("highway", StringComparison.OrdinalIgnoreCase)) {
                                street =
                                    street.Remove(street.IndexOf(matches[0].Value, StringComparison.OrdinalIgnoreCase),
                                                  matches[0].Length).Trim();
                                if (int.TryParse(matches[0].Value, out number)) {
                                    houseNumber = number;
                                }

                                break;
                            }
                        }

                        street = street.Remove(street.IndexOf(matches[0].Value, StringComparison.OrdinalIgnoreCase),
                                               matches[0].Length).Trim();
                        if (int.TryParse(matches[0].Value, out number)) {
                            houseNumber = number;
                        }

                        // if there are two check that the second is not at the very end and followed by a direction otherwise drop it
                        if (matches.Count == 2) {
                            // check for apartment or unit thing
                            var possibleUnitNumber = matches[1].Value;
                            var length = possibleUnitNumber.Length;
                            var index = street.LastIndexOf(possibleUnitNumber, StringComparison.OrdinalIgnoreCase);

                            if (street.EndsWith(possibleUnitNumber, StringComparison.OrdinalIgnoreCase)) {
                                street = street.Remove(index, length).Trim();

                                break;
                            }

                            var segment = street[index..].Trim();

                            // also check for ordinal numbers like the aves.
                            var ordinalStreetMatch = ordinalRegex.Matches(segment);
                            if (ordinalStreetMatch.Count > 0) {
                                streetName = ordinalStreetMatch[0].Value;

                                break;
                            }

                            // check that this is a direction - it's an acs address then
                            if (segment.Contains(' ')) {
                                var notTheNumber =
                                    segment.Remove(segment.IndexOf(possibleUnitNumber, StringComparison.OrdinalIgnoreCase),
                                                   possibleUnitNumber.Length).Trim();

                                if (directionRegex.IsMatch(notTheNumber)) {
                                    street = street.Remove(index, length).Trim();
                                    streetName = matches[1].Value;

                                    break;
                                }
                            }

                            // otherwise shit can it
                            street = street.Remove(index, segment.Length).Trim();

                            break;
                        }

                        // if there are three then throw out the last one since it's probably a unit
                        street = street.Remove(street.IndexOf(matches[1].Value, StringComparison.OrdinalIgnoreCase),
                                               matches[1].Length).Trim();
                        streetName = matches[1].Value;

                        street = street[..street.LastIndexOf(matches[2].Value, StringComparison.OrdinalIgnoreCase)].Trim();

                        break;
                    }
            }

            return ReplaceExtraCharacters(street);
        }
        private static string ParseStreetType(string street, bool isHighway, Regex streetTypeRegex, Dictionary<StreetType, string>? streetTypeAbbreviations, out StreetType streetType) {
            var matches = streetTypeRegex.Matches(street);
            streetType = StreetType.None;

            switch (matches.Count) {
                case 1: {
                        streetType = ParseStreetType(matches[0].Value, streetTypeAbbreviations);

                        if (streetType == StreetType.Highway && isHighway) {
                            break;
                        }

                        return street.Remove(matches[0].Index, matches[0].Length).Trim();
                    }
                case 2: {
                        //case where address has two street types in the name
                        //5301 W Jacob Hill Cir 84081
                        streetType = ParseStreetType(matches[1].Value, streetTypeAbbreviations);

                        return street.Remove(matches[1].Index, matches[1].Length).Trim();
                    }
            }

            return street;
        }
        private static StreetType ParseStreetType(string match, Dictionary<StreetType, string>? streetTypeAbbreviations) {
            var abbr = match.ToLower();

            if (Enum.TryParse(abbr, true, out StreetType streetType)) {
                return streetType;
            }

            if (streetTypeAbbreviations?.Values.Any(x => x.Split(',').Contains(abbr)) == true) {
                return streetTypeAbbreviations
                    .Where(x => x.Value.Split(',').Contains(abbr, new StreetTypeAbbreviationComparer()))
                    .Select(x => x.Key)
                    .SingleOrDefault();
            }

            return streetType;
        }
        private static string ParseDirections(string street, string streetName, StreetType streetType, string standardStreet, Regex directionRegex, Dictionary<StreetType, string>? streetTypeAbbreviations, out Direction prefixDirection, out Direction suffixDirection) {
            prefixDirection = Direction.None;
            suffixDirection = Direction.None;

            var matches = directionRegex.Matches(street);

            switch (matches.Count) {
                case 1: {
                        if (!IsOneCharacterStreetName(street, streetName, matches[0].Value)) {
                            var findLocation = matches[0].Index;

                            var averageWordCount = (street.Split(' ').Length - 1) / 2;

                            var wordLocation = GetWordIndex(findLocation, street);
                            if (wordLocation <= averageWordCount) {
                                if (TryParseDirection(matches[0].Value, out var direction)) {
                                    prefixDirection = direction;
                                }
                            } else {
                                if (TryParseDirection(matches[0].Value, out var direction)) {
                                    suffixDirection = direction;
                                }
                            }

                            street = street.Remove(matches[0].Index, matches[0].Length).Trim();
                        }

                        break;
                    }
                case 2: {
                        var words = standardStreet.Split(' ').ToList();
                        var indexOfMatch = words.IndexOf(matches[0].Value);

                        // parse out prefix direction
                        if (indexOfMatch - 1 >= 0 && !directionRegex.IsMatch(words[indexOfMatch - 1])) {
                            //if not do as normal
                            street = street.Remove(matches[0].Index, matches[0].Length).Trim();
                            if (TryParseDirection(matches[0].Value, out var direction)) {
                                prefixDirection = direction;
                            }

                            words = standardStreet.Split(' ').ToList();
                        }

                        // check out whats up with the second direction
                        indexOfMatch = words.IndexOf(matches[1].Value);
                        if (indexOfMatch - 1 >= 0 && !directionRegex.IsMatch(words[indexOfMatch - 1])) {
                            if (IsOneCharacterStreetName(street, streetName, matches[1].Value)) {
                                return street.Trim();
                            }

                            // if there is no item after the index then do as normal
                            if (indexOfMatch + 1 > words.Count - 1) {
                                street = street.Remove(street.LastIndexOf(matches[1].Value, StringComparison.Ordinal),
                                                       matches[1].Length).Trim();
                                if (TryParseDirection(matches[1].Value, out var direction)) {
                                    suffixDirection = direction;
                                }

                                return street.Trim();
                            }

                            var afterType = words[indexOfMatch + 1];
                            // if before street type then we might be in a street name scenario
                            var directionBefore = streetType == StreetType.None ||
                                streetTypeAbbreviations?.Values.Any(x => x.Split(',').Contains(afterType.ToLower())) == true;
                            var streetWithoutCandidate = street
                                                         .Remove(street.LastIndexOf(matches[1].Value, StringComparison.Ordinal),
                                                                 matches[1].Length).Trim();

                            var numeric = anyDigit().IsMatch(streetWithoutCandidate) ||
                                          string.IsNullOrEmpty(streetWithoutCandidate);

                            // if direction before street type and street name isn't numeric then it should be a part of the street name
                            if (numeric && directionBefore) {
                                street = street.Remove(street.LastIndexOf(matches[1].Value, StringComparison.Ordinal),
                                                       matches[1].Length).Trim();
                                if (TryParseDirection(matches[1].Value, out var direction)) {
                                    suffixDirection = direction;
                                }
                            }
                        }

                        break;
                    }
            }

            return street.Trim();
        }
        private static bool IsOneCharacterStreetName(string street, string streetName, string candidate) {
            // is the street name empty if we remove the direction?
            var candidateRemoved = street.Remove(
                street.IndexOf(candidate, StringComparison.OrdinalIgnoreCase),
                candidate.Length)
            .Trim();

            return string.IsNullOrEmpty(candidateRemoved) && string.IsNullOrEmpty(streetName);
        }
        private static string ParseStreetName(string street, string streetName) {
            street = street.Trim();

            if (string.IsNullOrEmpty(streetName)) {
                return street;
            }

            if (street != streetName) {
                return $"{street} {streetName}".Trim();
            }

            return street;
        }
        public static int GetWordIndex(int findLocation, string words) {
            var index = 0;
            if (words.Length >= findLocation) {
                index = words[..findLocation].Split(' ').Length;
            }

            return --index;
        }

        [GeneratedRegex("d")]
        private static partial Regex anyDigit();
        [GeneratedRegex("[ ]{2,}", RegexOptions.None)]
        private static partial Regex MultipleSpaces();
    }
}
