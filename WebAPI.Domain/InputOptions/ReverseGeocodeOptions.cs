namespace WebAPI.Domain.InputOptions
{
    public class ReverseGeocodeOptions : Options
    {
        public ReverseGeocodeOptions()
        {
            Distance = 5;
            WkId = 26912;
        }

        /// <summary>
        ///     Gets or sets the distance in meters from the point to find an address.
        /// </summary>
        /// <value>
        ///     The distance in meters.
        /// </value>
        public double Distance { get; set; }

        /// <summary>
        ///     Gets or sets the spatial reference well known id.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        public int WkId { get; set; }
    }
}