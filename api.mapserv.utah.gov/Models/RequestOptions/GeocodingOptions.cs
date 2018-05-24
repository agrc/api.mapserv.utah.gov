using api.mapserv.utah.gov.Models.Constants;

namespace api.mapserv.utah.gov.Models.RequestOptions
{
    /// <summary>
    ///     The options available for geocoding
    /// </summary>
    public class GeocodingOptions : OptionBase
    {
        public GeocodingOptions()
        {
            AcceptScore = 70;
            SuggestCount = 0;
            Locators = LocatorType.All;
            WkId = 26912;
            PoBox = true;
            ScoreDifference = false;
        }

        /// <summary>
        ///     Gets or sets the accept score for the address.
        /// </summary>
        /// <value>
        ///     The accept score defaults to 70 in the GeocodeOptionsModelBinder.
        /// </value>
        public int AcceptScore { get; set; }

        /// <summary>
        ///     Gets or sets the suggest count for how many address candidates to return.
        /// </summary>
        /// <value>
        ///     The suggest count.
        /// </value>
        public int SuggestCount { get; set; }

        /// <summary>
        ///     Gets or sets the locators.
        /// </summary>
        /// <value>
        ///     The locators to grab for geocoding.
        /// </value>
        public LocatorType Locators { get; set; }

        /// <summary>
        ///     Gets or sets the spatial reference well known id.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        public int WkId { get; set; }

        /// <summary>
        ///     Instructs the API to try to find the post office or delivery point for a post office address.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should geocode po box; otherwise, <c>false</c>.
        /// </value>
        public bool PoBox { get; set; }

        /// <summary>
        ///     Instructs the api to return the difference between the match address and the next best option. 
        ///     This is only calculated if this property is true and the suggest count is 0
        /// </summary>
        /// <value>
        ///   <c>true</c> if should calculate the sccore difference; otherwise, <c>false</c>.
        /// </value>
        public bool ScoreDifference { get; set; }

        public override string ToString() => $"AcceptScore: {AcceptScore}, SuggestCount: {SuggestCount}, JsonFormat: {JsonFormat}, Type: {Locators}";
    }
}