using System.ComponentModel;
using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Milepost {
    public class ReverseRouteMilepostRequestOptionsContract : ProjectableOptions {
        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        /// <value>
        /// The side.
        /// </value>
        [DefaultValue(100)]
        public double Buffer { get; set; } = 100;


        /// <summary>
        /// Gets or sets the spatial reference well known id.
        /// </summary>
        /// <value>
        ///     The wkid.
        /// </value>
        [DefaultValue(26912)]
        public int WkId { get; set; }

        /// <summary>
        /// Decides whether to include ramps
        /// </summary>
        [DefaultValue(false)]
        public bool IncludeRampSystem { get; set; } = false;

        /// <summary>
        ///     The count of candidates to return beside the highest match candidate.
        ///     Default: 0
        /// </summary>
        [DefaultValue(0)]
        public int Suggest { get; set; } = 0;
    }
}
