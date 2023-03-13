using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AGRC.api.Cache;
using AGRC.api.Comparers;
using AGRC.api.Infrastructure;
using AGRC.api.Models.Constants;
using Serilog;

namespace AGRC.api.Features.Geocoding {
    public class AddressParsing {
        public class Computation : IComputation<CleansedAddress> {
            public Computation(string street) {
                Street = street;
            }

            internal string Street { get; set; }
        }

        public class Handler : IComputationHandler<Computation, CleansedAddress> {
            private readonly IAbbreviations _abbreviations;
            private readonly ILogger _log;
            private readonly IRegexCache _regexCache;
            private string Street { get; set; }
            private string OriginalStreet { get; set; }
            private string StandardStreet { get; set; }

            public Handler(IRegexCache regexCache, IAbbreviations abbreviations, ILogger log) {
                _regexCache = regexCache;
                _abbreviations = abbreviations;
                _log = log?.ForContext<AddressParsing>();
            }

            public Task<CleansedAddress> Handle(Computation request, CancellationToken cancellationToken) {
                var street = request.Street.Replace(".", "").Replace(",", "").Replace("_", " ");
                Street = street;
                OriginalStreet = Street;

                var address = new CleansedAddress {
                    InputAddress = Street
                };

                Replacements(Street, address);
                ParsePoBox(Street, address);

                if (address.IsPoBox) {
                    return Task.FromResult(address);
                }

                ParseNumbers(Street, address);
                ParseStreetType(Street, address);
                ParseDirections(Street, address);
                ParseStreetName(Street, address);

                StandardStreet = address.StandardizedAddress;

                _log.ForContext("original", OriginalStreet)
                    .ForContext("standardized", StandardStreet)
                    .Information("address parsing complete");

                return Task.FromResult(address);
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

            public bool IsOneCharacterStreetName(AddressBase address, string candidate) {
                // is the street name empty if we remove the direction?
                var candidateRemoved = Street
                                       .Remove(Street.IndexOf(candidate, StringComparison.OrdinalIgnoreCase),
                                               candidate.Length).Trim();

                return string.IsNullOrEmpty(candidateRemoved) && string.IsNullOrEmpty(address.StreetName);
            }

            public static int GetWordIndex(int findLocation, string words) {
                var index = 0;
                if (words.Length >= findLocation) {
                    index = words[..findLocation].Split(' ').Length;
                }

                return --index;
            }

            private void Replacements(string street, AddressBase address) {
                ReplaceHighway(street, address);
                ReplaceUnitTypes(street);
                ReplaceDirections(street);
            }

            private void ParsePoBox(string street, AddressBase address) {
                var match = _regexCache.Get("pobox").Match(street);
                if (!match.Success) {
                    address.IsPoBox = false;

                    return;
                }

                var pobox = match.Groups[1].Value;
                if (!int.TryParse(pobox, out var poboxValue)) {
                    address.IsPoBox = false;

                    return;
                }

                address.StreetName = "P.O. Box";
                address.PoBox = poboxValue;
                address.IsPoBox = true;
            }

            private void ReplaceHighway(string street, AddressBase address) {
                if (!_regexCache.Get("highway").IsMatch(street)) {
                    return;
                }

                Street = _regexCache.Get("highway").Replace(street, "Highway");
                address.IsHighway = true;
            }

            private void ReplaceUnitTypes(string street) {
                var matches = _regexCache.Get("unitType").Matches(street);

                //probably a secondary address
                if (street.Contains('#')) {
                    var indexOfValue = "#";

                    //check to see if the # is preceded by a secondary address unit type
                    var unitMatches = _regexCache.Get("unitTypeLookBehind").Matches(street);
                    if (unitMatches.Count > 0) {
                        indexOfValue = unitMatches[0].Value;
                    }

                    var index = street.LastIndexOf(indexOfValue, StringComparison.OrdinalIgnoreCase);
                    street = street[..index];

                    Street = street;
                }

                if (matches.Count == 0) {
                    return;
                }

                //get last match since street name could be in there?
                var match = matches[^1].Value.ToLower();

                var unitType = _abbreviations.UnitAbbreviations.Single(x => x.Item1 == match || x.Item2 == match);

                if (!unitType.Item3) {
                    // unit doesn't need a number after it. check that it is at
                    // the end of the street and remove it if it is.

                    if (!street.EndsWith(match, StringComparison.OrdinalIgnoreCase)) {
                        return;
                    }

                    var index = street.LastIndexOf(match, StringComparison.OrdinalIgnoreCase);
                    street = string.Concat(street.AsSpan(0, index), street.AsSpan(index + match.Length));

                    Street = street;

                    return;
                }

                //make sure address has a number after it since it's required.
                var regex = new Regex(string.Format(@"{0}(?:\s|.)?([a-z]?(?:\d)*[a-z]?)", match),
                                      RegexOptions.IgnoreCase);
                var moreMatches = regex.Matches(street);

                if (moreMatches.Count > 0 &&
                    street.EndsWith(moreMatches[^1].Value, StringComparison.OrdinalIgnoreCase)) {
                    var theMatch = moreMatches[^1];
                    var index = street.LastIndexOf(theMatch.Value, StringComparison.OrdinalIgnoreCase);
                    street = string.Concat(street.AsSpan(0, index), street.AsSpan(index + theMatch.Length));

                    Street = street;
                }
            }

            private void ReplaceDirections(string street) {
                var match = _regexCache.Get("directionSubstitutions").Match(street);
                if (!match.Success) {
                    return;
                }

                static string Replace(string original, string matchedValue, string replaced) => original.Replace(matchedValue, replaced);

                while (match.Success) {
                    var value = match.Value;
                    switch (value.ToLowerInvariant()[0]) {
                        case 'n':
                            Street = Replace(street, value, "north");
                            break;
                        case 's':
                            Street = Replace(street, value, "south");
                            break;
                        case 'e':
                            Street = Replace(street, value, "east");
                            break;
                        case 'w':
                            Street = Replace(street, value, "west");
                            break;
                    }

                    match = match.NextMatch();
                }
            }

            private void ParseNumbers(string street, AddressBase address) {
                //make space between 500west = 500 west
                street = Street = StandardStreet = _regexCache.Get("separateNameAndDirection")
                                                              .Replace(street, "$1 $2");

                var matches = _regexCache.Get("streetNumbers").Matches(street);
                int houseNumber;

                switch (matches.Count) {
                    case 1: {
                            Street = Street.Remove(Street.IndexOf(matches[0].Value, StringComparison.OrdinalIgnoreCase),
                                                   matches[0].Length);
                            if (int.TryParse(matches[0].Value, out houseNumber)) {
                                address.HouseNumber = houseNumber;
                            }

                            break;
                        }
                    case 2:
                    case 3: {
                            if (address.IsHighway) {
                                var parts = Street.Split(' ').ToList();
                                var numberIndex = parts.IndexOf(matches[1].Value);

                                var highwayIndex = numberIndex - 1;

                                if (highwayIndex > -1 && parts[highwayIndex].Contains("highway", StringComparison.OrdinalIgnoreCase)) {
                                    Street =
                                        Street.Remove(Street.IndexOf(matches[0].Value, StringComparison.OrdinalIgnoreCase),
                                                      matches[0].Length);
                                    if (int.TryParse(matches[0].Value, out houseNumber)) {
                                        address.HouseNumber = houseNumber;
                                    }

                                    address.IsHighway = true;
                                    //not street number but highway.
                                    break;
                                }
                            }

                            Street = Street.Remove(Street.IndexOf(matches[0].Value, StringComparison.OrdinalIgnoreCase),
                                                   matches[0].Length);
                            if (int.TryParse(matches[0].Value, out houseNumber)) {
                                address.HouseNumber = houseNumber;
                            }

                            //if there are two check that the second is not at the very end and followed by a direction otherwise drop it
                            if (matches.Count == 2) {
                                //check for apartment or unit thing
                                var possibleUnitNumber = matches[1].Value;
                                var length = possibleUnitNumber.Length;
                                var index = Street.LastIndexOf(possibleUnitNumber, StringComparison.OrdinalIgnoreCase);

                                if (Street.EndsWith(possibleUnitNumber, StringComparison.OrdinalIgnoreCase)) {
                                    Street = Street.Remove(index, length).Trim();

                                    break;
                                }

                                var segment = Street[index..].Trim();

                                // also check for ordinal numbers like the aves.
                                var ordinalStreetMatch = _regexCache.Get("ordinal").Matches(segment);
                                if (ordinalStreetMatch.Count > 0) {
                                    segment = ordinalStreetMatch[0].Value;

                                    address.StreetName = segment;

                                    break;
                                }

                                // check that this is a direction - it's an acs address then
                                if (segment.Contains(' ')) {
                                    var notTheNumber =
                                        segment.Remove(
                                                       segment.IndexOf(possibleUnitNumber,
                                                                       StringComparison.OrdinalIgnoreCase),
                                                       possibleUnitNumber.Length);

                                    if (_regexCache.Get("direction").IsMatch(notTheNumber)) {
                                        Street = Street.Remove(index, length);
                                        address.StreetName = matches[1].Value;

                                        break;
                                    }
                                }

                                // otherwise shit can it
                                Street = Street.Remove(index, segment.Length).Trim();

                                break;
                            }

                            //if there are three then throw out the last one since it's probably a unit
                            Street = Street.Remove(Street.IndexOf(matches[1].Value, StringComparison.OrdinalIgnoreCase),
                                                   matches[1].Length);
                            address.StreetName = matches[1].Value;

                            Street = Street[..Street.LastIndexOf(matches[2].Value, StringComparison.OrdinalIgnoreCase)];

                            break;
                        }
                }
            }

            private void ParseStreetType(string street, AddressBase address) {
                var matches = _regexCache.Get("streetType").Matches(street);

                switch (matches.Count) {
                    case 1: {
                            var type = ParseStreetType(matches[0].Value);

                            if (type == StreetType.Highway && address.IsHighway) {
                                break;
                            }

                            address.StreetType = type;

                            Street = Street.Remove(matches[0].Index, matches[0].Length);

                            break;
                        }
                    case 2: {
                            //case where address has two street types in the name
                            //5301 W Jacob Hill Cir 84081
                            address.StreetType = ParseStreetType(matches[1].Value);

                            Street = Street.Remove(matches[1].Index, matches[1].Length);

                            break;
                        }
                }
            }

            private static void ParseStreetName(string street, AddressBase address) {
                street = street.Trim();
                if (string.IsNullOrEmpty(street)) {
                    return;
                }

                if (string.IsNullOrEmpty(address.StreetName)) {
                    address.StreetName = street;
                }

                if (street != address.StreetName) {
                    address.StreetName = street + " " + address.StreetName;
                }
            }

            private StreetType ParseStreetType(string match) {
                var abbr = match.ToLower();

                if (Enum.TryParse(abbr, true, out StreetType streetType)) {
                    return streetType;
                }

                if (_abbreviations.StreetTypeAbbreviations?.Values.Any(x => x.Split(',').Contains(abbr)) == true) {
                    return
                        _abbreviations.StreetTypeAbbreviations
                                      .Where(x => x.Value.Split(',')
                                                   .Contains(abbr, new StreetTypeAbbreviationComparer()))
                                      .Select(x => x.Key)
                                      .SingleOrDefault();
                }

                return streetType;
            }

            private void ParseDirections(string street, AddressBase address) {
                Direction dir;

                var matches = _regexCache.Get("direction").Matches(street);

                switch (matches.Count) {
                    case 1: {
                            if (!IsOneCharacterStreetName(address, matches[0].Value)) {
                                var findLocation = matches[0].Index;

                                var averageWordCount = (street.Split(' ').Length - 1) / 2;

                                var wordLocation = GetWordIndex(findLocation, street);
                                if (wordLocation <= averageWordCount) {
                                    address.PrefixDirection = TryParseDirection(matches[0].Value, out dir)
                                        ? dir
                                        : Direction.None;
                                } else {
                                    address.SuffixDirection = TryParseDirection(matches[0].Value, out dir)
                                        ? dir
                                        : Direction.None;
                                }

                                Street = street.Remove(matches[0].Index, matches[0].Length);
                            }

                            break;
                        }
                    case 2: {
                            var words = StandardStreet.Split(' ').ToList();
                            var indexOfMatch = words.IndexOf(matches[0].Value);

                            // parse out prefix direction
                            if (indexOfMatch - 1 >= 0 && !_regexCache.Get("direction").IsMatch(words[indexOfMatch - 1])) {
                                //if not do as normal
                                Street = street.Remove(matches[0].Index, matches[0].Length);
                                address.PrefixDirection = TryParseDirection(matches[0].Value, out dir)
                                    ? dir
                                    : Direction.None;

                                words = StandardStreet.Split(' ').ToList();
                            }

                            // check out whats up with the second direction
                            indexOfMatch = words.IndexOf(matches[1].Value);
                            if (indexOfMatch - 1 >= 0 && !_regexCache.Get("direction").IsMatch(words[indexOfMatch - 1])) {
                                if (IsOneCharacterStreetName(address, matches[1].Value)) {
                                    return;
                                }

                                // if there is no item after the index then do as normal
                                if (indexOfMatch + 1 > words.Count - 1) {
                                    Street = Street.Remove(Street.LastIndexOf(matches[1].Value, StringComparison.Ordinal),
                                                           matches[1].Length);
                                    address.SuffixDirection = TryParseDirection(matches[1].Value, out dir)
                                        ? dir
                                        : Direction.None;

                                    return;
                                }

                                var afterType = words[indexOfMatch + 1];
                                // if before street type then we might be in a street name scenario
                                var directionBefore =
                                    _abbreviations.StreetTypeAbbreviations.Values.Any(x => x.Split(',')
                                                                                            .Contains(afterType
                                                                                                          .ToLower())) ||
                                    address.StreetType == StreetType.None;
                                var streetWithoutCandidate = Street
                                                             .Remove(Street.LastIndexOf(matches[1].Value, StringComparison.Ordinal),
                                                                     matches[1].Length).Trim();

                                var numeric = new Regex("[0-9]").IsMatch(streetWithoutCandidate) ||
                                              string.IsNullOrEmpty(streetWithoutCandidate);

                                // if direction before street type and street name isn't numeric then it should be a part of the street name
                                if (numeric && directionBefore) {
                                    Street = Street.Remove(Street.LastIndexOf(matches[1].Value, StringComparison.Ordinal),
                                                           matches[1].Length);
                                    address.SuffixDirection = TryParseDirection(matches[1].Value, out dir)
                                        ? dir
                                        : Direction.None;
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}
