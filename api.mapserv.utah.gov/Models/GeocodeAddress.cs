using System.Collections.Generic;

namespace api.mapserv.utah.gov.Models
{
  public class GeocodeAddress : CleansedAddress
  {
      public GeocodeAddress(AddressBase address)
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
                )
      { }

      public List<GridLinkable> AddressGrids { get; set; }

      public override string ToString() => $"[GeocodeAddress] InputAddress: {InputAddress}, Zip5: {Zip5}";
  }
}
