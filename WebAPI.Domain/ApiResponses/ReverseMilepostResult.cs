using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace WebAPI.Domain.ApiResponses
{
    public class ReverseMilepostResults
    {
        private string _routeName;
        private double _distance;
        private double _milepost;

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        /// <value>
        /// The name of the route with the leading zeros removed.
        /// </value>
        [JsonProperty(PropertyName = "route")]
        public string Route
        {
            get
            {
                if (string.IsNullOrEmpty(_routeName))
                {
                    return "";
                }

                return _routeName.TrimStart('0');
            }
            set => _routeName = value;
        }

        /// <summary>
        ///     Gets or sets the distance away from the input point.
        /// </summary>
        /// <value>
        ///     The distance away from the input point in meters. -1 for not found. Rounded to two decimal places
        /// </value>
        [JsonProperty(PropertyName = "offsetMeters")]
        public double OffsetMeters
        {
            get => Math.Round(_distance, 2);
            set => _distance = value;
        }

        /// <summary>
        ///     Gets or sets the milepost.
        /// </summary>
        /// <value>
        ///     The closest milepost value rounded to three decimal places.
        /// </value>
        [JsonProperty(PropertyName = "milepost")]
        public double Milepost
        {
            get => Math.Round(_milepost, 3);
            set => _milepost = value;
        }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        /// <value>
        /// The side of the road that the point was on.
        /// </value>
        [JsonProperty(PropertyName = "side")]
        public string Side => Increasing ? "increasing" : "decreasing";

        /// <summary>
        /// Gets or sets a value indicating whether [increasing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [increasing]; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool Increasing { get; set; }
    }

    public class ReverseMilepostResult
    {
        private string _routeName;
        private double _distance;
        private double _milepost;

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        /// <value>
        /// The name of the route with the leading zeros removed.
        /// </value>
        [JsonProperty(PropertyName = "route")]
        public string Route
        {
            get
            {
                if (string.IsNullOrEmpty(_routeName))
                {
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
        [JsonProperty(PropertyName = "offsetMeters")]
        public double OffsetMeters
        {
            get => Math.Round(_distance, 2);
            set => _distance = value;
        }

        /// <summary>
        ///     Gets or sets the milepost.
        /// </summary>
        /// <value>
        ///     The closest milepost value rounded to three decimal places.
        /// </value>
        [JsonProperty(PropertyName = "milepost")]
        public double Milepost
        {
            get => Math.Round(_milepost, 3);
            set => _milepost = value;
        }

        /// <summary>
        /// Gets or sets the side.
        /// </summary>
        /// <value>
        /// The side of the road that the point was on.
        /// </value>
        [JsonProperty(PropertyName = "side")]
        public string Side => Increasing ? "increasing" : "decreasing";

        [JsonProperty(PropertyName = "dominant")]
        public bool? Dominant { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [increasing].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [increasing]; otherwise, <c>false</c>.
        /// </value>
        [JsonIgnore]
        public bool Increasing { get; set; }

        [JsonProperty(PropertyName = "candidates")]
        public IEnumerable<ReverseMilepostResult> Candidates { get; set; }

        public bool ShouldSerializeCandidates()
        {
            if (Candidates == null)
                return false;

            return Candidates.Any();
        }
    }
}