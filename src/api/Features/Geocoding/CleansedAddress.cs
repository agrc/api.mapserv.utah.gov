using AGRC.api.Models.Constants;

namespace AGRC.api.Features.Geocoding;
public class CleansedAddress : AddressBase {
    public CleansedAddress() {
    }

    public CleansedAddress(string inputAddress, int? houseNumber, double milepost, int poBox,
                           Direction prefixDirection, string streetName, StreetType streetType,
                           Direction suffixDirection, int zip4, int? zip5, bool isHighway, bool isPobox) :
        base(inputAddress,
             houseNumber,
             milepost,
             poBox,
             prefixDirection,
             streetName,
             streetType,
             suffixDirection,
             zip4,
             zip5,
             isHighway,
             isPobox) {
    }

    /// <summary>
    ///     Gets or sets the standardized address.
    /// </summary>
    /// <value>
    ///     The standardized address that we modify to fix common address issues.
    ///     US89 => Highway89
    ///     1991N => 1991 N
    /// </value>
    // public string StandardizedAddress { get; set; }
    public string StandardizedAddress {
        get {
            if (IsPoBox) {
                return $"P.O. Box {PoBox}";
            }

            var address = $"{HouseNumber} {PrefixDirection} {StreetName} {StreetType} {SuffixDirection}";

            address = address.Replace("None", "");

            var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
            address = regex.Replace(address, " ");

            return address.Trim();
        }
    }
}
