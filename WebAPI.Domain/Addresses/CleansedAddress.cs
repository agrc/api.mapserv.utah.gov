using System;
using System.Text.RegularExpressions;

namespace WebAPI.Domain.Addresses
{
    [Serializable]
    public class CleansedAddress : AddressBase
    {
        public CleansedAddress()
        {
            
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
                                      isPobox)
        {

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

        public string StandardizedAddress
        {
            get
            {
                if (IsPoBox)
                {
                    return string.Format("P.O. Box {0}", PoBox);
                }
                var address = string.Format("{0} {1} {2} {4} {3}", HouseNumber, PrefixDirection, StreetName,
                                            SuffixDirection, StreetType);

                address = address.Replace("None", "");

                var regex = new Regex(@"[ ]{2,}", RegexOptions.None);
                address = regex.Replace(address, @" ");

                return address.Trim();
            }
        }

        public override string ToString()
        {
            return string.Format("[Address] InputAddress: {0}, Zip5: {1}", InputAddress, Zip5);
        }
    }
}