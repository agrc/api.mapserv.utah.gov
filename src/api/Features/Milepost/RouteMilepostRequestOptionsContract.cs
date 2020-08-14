using AGRC.api.Models.RequestOptionContracts;

namespace AGRC.api.Features.Milepost {
    public class RouteMilepostRequestOptionsContract : ProjectableOptions {
        /// <summary>
        /// The side of a divided highway.
        /// </summary>
        /// <example>
        /// increasing
        /// </example>
        public SideDelineation Side { get; set; } = SideDelineation.Increasing;

        /// <summary>
        ///  Decides whether the route is the highway only or the highway + direction + milepost from the data
        /// </summary>
        /// <example>
        /// 0015PC30554
        /// </example>
        public bool FullRoute { get; set; }

        public override string ToString() => $"Side: {Side}. Use FullRoute {FullRoute}";
    }
}
