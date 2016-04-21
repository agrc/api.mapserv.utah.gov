using System;
using System.Collections.Generic;
using WebAPI.Domain.Linkers;

namespace WebAPI.Domain.Addresses
{
    [Serializable]
    public class GeocodeAddress : CleansedAddress
    {
        public GeocodeAddress(CleansedAddress address)
            : base(address.InputAddress,
                   address.HouseNumber,
                   address.Milepost,
                   address.PoBox,
                   address.PrefixDirection,
                   address.StreetName,
                   address.StreetType,
                   address.SuffixDirection,
                   address.Zip4,
                   address.Zip5,
                   address.IsHighway,
                   address.IsPoBox
            ) {}

        public List<GridLinkable> AddressGrids { get; set; }

        public override string ToString()
        {
            return string.Format("[GeocodeAddress] InputAddress: {0}, Zip5: {1}", InputAddress, Zip5);
        }
    }
}