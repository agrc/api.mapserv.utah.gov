using System;
using System.Collections.Generic;
using AGRC.api.Features.Geocoding;
using AGRC.api.Models.Constants;
using AGRC.api.Models.Linkables;

namespace api.tests.Features.Geocoding;

public static class AddressHelper {
    public static Address CreateEmptyAddress() {
        var inputAddress = string.Empty;
        int? houseNumber = null;
        const Direction prefixDirection = Direction.None;
        var streetName = string.Empty;
        const StreetType streetType = StreetType.None;
        const Direction suffixDirection = Direction.None;
        const int zip4 = 0;
        const int zip5 = 0;
        var grids = Array.Empty<GridLinkable>();
        const int poBox = 0;
        const bool isPoBox = false;
        const bool isHighway = false;

        return new(inputAddress, houseNumber, prefixDirection, streetName, streetType,
                     suffixDirection, zip4, zip5, grids, poBox, isPoBox, isHighway);
    }
}
public static class AddressExtensions {
    public static Address CreateAddress(this IReadOnlyCollection<GridLinkable> grids) =>
        AddressHelper.CreateEmptyAddress().SetAddressGrids(grids);
}
