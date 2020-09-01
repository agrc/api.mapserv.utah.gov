using System;
using System.Collections.Generic;
using System.Linq;

namespace AGRC.api.Features.Milepost {
    public class ReverseRouteMilepostResponseContract {
        private string _routeName;
        private double _distance;
        private double _milepost;

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        /// <value>
        /// The name of the route with the leading zeros removed.
        /// </value>
        public string Route {
            get {
                if (string.IsNullOrEmpty(_routeName)) {
                    return "";
                }

                return _routeName.TrimStart('0').TrimEnd('M');
            }
            set => _routeName = value;
        }

        /// <summary>
        ///     Gets or sets the distance away from the input point.
        /// </summary>
        /// <value>
        ///     The distance away from the input point in meters. -1 for not found. Rounded to two decimal places
        /// </value>
        public double OffsetMeters {
            get => Math.Round(_distance, 2);
            set => _distance = value;
        }

        /// <summary>
        ///     Gets or sets the milepost.
        /// </summary>
        /// <value>
        ///     The closest milepost value rounded to three decimal places.
        /// </value>
        public double Milepost {
            get => Math.Round(_milepost, 3);
            set => _milepost = value;
        }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        /// <value>
        /// The side of the road that the point was on.
        /// </value>
        public string Side => _routeName.Contains('P', StringComparison.InvariantCultureIgnoreCase)
            ? "increasing" : "decreasing";

        public bool? Dominant { get; set; }

        public IEnumerable<ReverseRouteMilepostResponseContract> Candidates { get; set; }

        public bool ShouldSerializeCandidates() {
            if (Candidates == null)
                return false;

            return Candidates.Any();
        }
    }
}
