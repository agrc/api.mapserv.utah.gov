using System.ComponentModel;

namespace AGRC.api.Models.RequestOptions {
    public class ReverseGeocodingOptions : ProjectableOptions {
        /// <summary>
        ///     The distance in meters from the input location to look for an address.
        ///     Default: 5
        /// </summary>
        [DefaultValue(5)]
        public double Distance { get; set; } = 5;
    }
}
