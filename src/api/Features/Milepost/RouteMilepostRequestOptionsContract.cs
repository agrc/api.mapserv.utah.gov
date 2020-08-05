using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Milepost {
    public class RouteMilepostRequestOptionsContract : ProjectableOptions {
        /// <summary>
        /// Gets or sets the side of a divided highway.
        /// </summary>
        /// <value>
        /// The side.
        /// </value>
        public SideDelineation Side { get; set; } = SideDelineation.P;

        /// <summary>
        ///  Decides whether the route is the highway only or the highway + direction + milepost from the data
        /// </summary>
        public bool FullRoute { get; set; }

        public override string ToString() => $"Side: {Side}. Use FullRoute {FullRoute}";
    }
}
