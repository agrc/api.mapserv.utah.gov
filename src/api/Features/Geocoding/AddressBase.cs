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

    public bool IsHighway { get; }
    public bool IsPoBox { get; }

    public static Address BuildPoBoxAddress(string inputAddress, int poBox, int zip5, IReadOnlyCollection<GridLinkable>? grids = null) =>
         new(inputAddress, null, Direction.None, "P.O. Box", StreetType.None,
             Direction.None, zip5, 0, grids, poBox, true, false);

    [GeneratedRegex("[ ]{2,}", RegexOptions.None)]
    internal static partial Regex MultipleSpaces();
    [GeneratedRegex("[^05]$")]
    internal static partial Regex NotZeroOrFive();
    [GeneratedRegex("\\band\\b", RegexOptions.IgnoreCase, "en-US")]
    internal static partial Regex Intersection();
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

    public static bool HasPrefix(this Address address) => address.PrefixDirection != Direction.None;
    public static bool IsNumericStreetName(this Address address) {
        if (string.IsNullOrEmpty(address.StreetName)) {
            return false;
        }

        return int.TryParse(address.StreetName, out _);
    }
    public static bool IsReversal(this Address address) {
        if (string.IsNullOrEmpty(address.StreetName)) {
            return false;
        }

        if (!address.IsNumericStreetName()) {
            return false;
        }

        var notZeroOrFive = Address.NotZeroOrFive();

        return notZeroOrFive.IsMatch(address.StreetName);
    }
    public static bool PossibleReversal(this Address address) {
        if (string.IsNullOrEmpty(address.StreetName)) {
            return false;
        }

        var stepOne = false;

        if (int.TryParse(address.StreetName, out var num)) {
            stepOne = num % 5 == 0;
        }

        var stepTwo = address.HouseNumber % 5 == 0;

        return stepOne && stepTwo;
    }
    public static bool IsIntersection(this Address address) {
        var regex = Address.Intersection();

        return regex.IsMatch(address.InputAddress);
    }
    public static bool IsMatchable(this Address address) {
        if (address.IsIntersection()) {
            return true;
        }

        if (!address.HouseNumber.HasValue) {
            return address.SuffixDirection != Direction.None;
        }

        return true;
    }
    public static string ReverseAddressParts(this Address address) {
        var reversed = string.Format("{2} {3} {4} {0} {1}", address.HouseNumber, address.PrefixDirection, address.StreetName,
                                    address.SuffixDirection, address.StreetType);

        reversed = reversed.Replace("None", "");

        var regex = Address.MultipleSpaces();
        reversed = regex.Replace(reversed, " ");

        return reversed.Trim();
    }

    /// <summary>
    ///     Gets or sets the standardized address.
    /// </summary>
    /// <value>
    ///     The standardized address that we modify to fix common address issues.
    ///     US89 => Highway89
    ///     1991N => 1991 N
    /// </value>
    public static string StandardizedAddress(this Address address) {
        if (address.IsPoBox) {
            return $"P.O. Box {address.PoBox}";
        }

        var standardPattern = $"{address.HouseNumber} {address.PrefixDirection} {address.StreetName} {address.StreetType} {address.SuffixDirection}";

        if (address.IsHighway) {
            standardPattern = $"{address.HouseNumber} {address.PrefixDirection} {address.StreetName} {address.SuffixDirection}";
        }

        standardPattern = standardPattern.Replace("None", "");

        var regex = Address.MultipleSpaces();
        standardPattern = regex.Replace(standardPattern, " ");

        return standardPattern.Trim();
    }
}
