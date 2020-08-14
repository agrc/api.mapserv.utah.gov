using System.ComponentModel;
using AGRC.api.Models.Constants;
using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Geocoding {
    /// <summary>
    /// The options available for geocoding
    /// </summary>
    public class SingleGeocodeRequestOptionsContract : ProjectableOptions {
        /// <summary>
        /// A score from 0 to 100 used to rank geocode candidates.
        /// </summary>
        [DefaultValue(70)]
        public int AcceptScore { get; set; } = 70;

        /// <summary>
        /// The count of candidates to return beside the highest match candidate.
        /// </summary>
        /// <example>
        /// 5
        /// </example>
        [DefaultValue(0)]
        public int Suggest { get; set; } = 0;

        /// <summary>
        /// How the service will attempt to locate the address.
        /// All is a combination of address point locators and road centerline locators; This will offer the best results.
        /// addressPoints will only geocode on address points and roadCenterlines will only geocode on road centerlines.
        /// </summary>
        [DefaultValue(LocatorType.All)]
        public LocatorType Locators { get; set; } = LocatorType.All;

        /// <summary>
        /// How the service will handle P.O. Boxes. True will return the coordinates of the post office where the mail is
        /// delivered. False will return no match. P.O. Boxes can only be processed if the zone is a zip code. If a place name
        /// is used in the zone the geocode will return no match.
        /// </summary>
        [DefaultValue(true)]
        public bool PoBox { get; set; } = true;

        /// <summary>
        /// Request the api to calculate the difference in score between the match address and the top suggestion. This flag is
        /// only calculated and returned when `suggest=0` and there is a top suggestion. If there is no top suggestion, the
        /// property is not sent. If the value is 0, then you have a tie and should investigate the addresses by using the
        /// suggest items.
        /// </summary>
        [DefaultValue(false)]
        public bool ScoreDifference { get; set; } = false;
    }
}
