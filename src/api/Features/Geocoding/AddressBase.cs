using AGRC.api.Models.Constants;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Features.Geocoding;
public partial class Address {
    public Address(string inputAddress, int? houseNumber,
                 Direction prefixDirection, string streetName,
                 StreetType streetType, Direction suffixDirection,
                 int? zip5, int zip4,
                 IReadOnlyCollection<GridLinkable>? addressGrids,
                 int poBox, bool isPoBox, bool isHighway) {
        InputAddress = inputAddress;
        HouseNumber = houseNumber;
        PrefixDirection = prefixDirection;
        StreetName = streetName;
        StreetType = streetType;
        SuffixDirection = suffixDirection;
        Zip5 = zip5;
        Zip4 = zip4;
        AddressGrids = addressGrids ?? Array.Empty<GridLinkable>();
        PoBox = poBox;
        IsPoBox = isPoBox;
        IsHighway = isHighway;
    }

    public string InputAddress { get; }

    public int? HouseNumber { get; }
    public Direction PrefixDirection { get; }
    public string StreetName { get; }
    public StreetType StreetType { get; }
    public Direction SuffixDirection { get; }

    public int PoBox { get; }
    public int Zip4 { get; }
    public int? Zip5 { get; }

    public IReadOnlyCollection<GridLinkable> AddressGrids { get; }
    /// <summary>
    ///     Gets or sets the standardized address.
    /// </summary>
    /// <value>
    ///     The standardized address that we modify to fix common address issues.
    ///     US89 => Highway89
    ///     1991N => 1991 N
    /// </value>
    public string StandardizedAddress {
        get {
            if (IsPoBox) {
                return $"P.O. Box {PoBox}";
            }

            var standardPattern = $"{HouseNumber} {PrefixDirection} {StreetName} {StreetType} {SuffixDirection}";

            if (IsHighway) {
                standardPattern = $"{HouseNumber} {PrefixDirection} {StreetName} {SuffixDirection}";
            }

            standardPattern = standardPattern.Replace("None", "");

            var regex = MultipleSpaces();
            standardPattern = regex.Replace(standardPattern, " ");

            return standardPattern.Trim();
        }
    }
    public bool IsHighway { get; }

    public bool IsPoBox { get; }

    public string ReversalAddress {
        get {
            var address = string.Format("{2} {3} {4} {0} {1}", HouseNumber, PrefixDirection, StreetName,
                                        SuffixDirection, StreetType);

            address = address.Replace("None", "");

            var regex = MultipleSpaces();
            address = regex.Replace(address, " ");

            return address.Trim();
        }
    }

    public bool IsNumericStreetName() {
        if (string.IsNullOrEmpty(StreetName)) {
            return false;
        }

        return int.TryParse(StreetName, out _);
    }

    public bool HasPrefix() => PrefixDirection != Direction.None;

    public bool IsReversal() {
        if (string.IsNullOrEmpty(StreetName)) {
            return false;
        }

        if (!IsNumericStreetName()) {
            return false;
        }

        var notZeroOrFive = NotZeroOrFive();

        return notZeroOrFive.IsMatch(StreetName);
    }

    public bool PossibleReversal() {
        if (string.IsNullOrEmpty(StreetName)) {
            return false;
        }

        var stepOne = false;

        if (int.TryParse(StreetName, out var num)) {
            stepOne = num % 5 == 0;
        }

        var stepTwo = HouseNumber % 5 == 0;

        return stepOne && stepTwo;
    }

    public bool IsIntersection() {
        var regex = Intersection();

        return regex.IsMatch(InputAddress);
    }

    public bool IsMatchable() {
        if (IsIntersection()) {
            return true;
        }

        if (!HouseNumber.HasValue) {
            return SuffixDirection != Direction.None;
        }

        return true;
    }

    public static Address BuildPoBoxAddress(string inputAddress, int poBox, int zip5, IReadOnlyCollection<GridLinkable>? grids = null) =>
         new(inputAddress, null, Direction.None, "P.O. Box", StreetType.None,
             Direction.None, zip5, 0, grids, poBox, true, false);

    [GeneratedRegex("[ ]{2,}", RegexOptions.None)]
    private static partial Regex MultipleSpaces();
    [GeneratedRegex("[^05]$")]
    private static partial Regex NotZeroOrFive();
    [GeneratedRegex("\\band\\b", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex Intersection();
}

public static class AddressExtensions {
    public static Address SetAddressGrids(this Address address, IReadOnlyCollection<GridLinkable> grids) =>
         new(address.InputAddress, address.HouseNumber, address.PrefixDirection, address.StreetName,
                   address.StreetType, address.SuffixDirection, address.Zip5, address.Zip4, grids,
                   address.PoBox, address.IsPoBox, address.IsHighway);
    public static Address SetZipCodes(this Address address, int zip5, int zip4) =>
         new(address.InputAddress, address.HouseNumber, address.PrefixDirection, address.StreetName,
                   address.StreetType, address.SuffixDirection, zip5, zip4, address.AddressGrids,
                   address.PoBox, address.IsPoBox, address.IsHighway);
    public static Address SetPrefixDirection(this Address address, Direction direction) =>
         new(address.InputAddress, address.HouseNumber, direction, address.StreetName,
                   address.StreetType, address.SuffixDirection, address.Zip5, address.Zip4,
                   address.AddressGrids, address.PoBox, address.IsPoBox, address.IsHighway);
}
