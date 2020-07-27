using System.Collections.Generic;
using AGRC.api.Models.Linkables;

namespace AGRC.api.Models
{
  public class AddressWithGrids : CleansedAddress
  {
      public AddressWithGrids(AddressBase address)
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

      public IReadOnlyCollection<GridLinkable> AddressGrids { get; set; }
  }
}
