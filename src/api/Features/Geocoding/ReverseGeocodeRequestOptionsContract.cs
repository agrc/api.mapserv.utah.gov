using System.ComponentModel;
using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Geocoding {
    public class ReverseGeocodeRequestOptionsContract : ProjectableOptions {
        /// <summary>
        ///     The distance in meters from the input location to look for an address.
        ///     Default: 5
        /// </summary>
        [DefaultValue(5)]
        public double Distance { get; set; } = 5;
    }
}
