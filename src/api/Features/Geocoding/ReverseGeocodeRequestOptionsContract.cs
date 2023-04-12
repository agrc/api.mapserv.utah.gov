using System.ComponentModel;
using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Geocoding;
public class ReverseGeocodeRequestOptionsContract : ProjectableOptions {
    /// <summary>
    /// The distance in meters from the input location to look for an address.
    /// </summary>
    /// <example>
    /// 20
    /// </example>
    [DefaultValue(5)]
    public double Distance { get; set; } = 5;
}
