using System.Collections.Generic;

namespace WebAPI.Domain.ArcServerResponse.Soe
{
    public class ReverseMilepostResponse : SoeErrorable
    {
        public List<ReverseMilepostResult> Results { get; set; }

        public class ReverseMilepostResult
        {
            public string RouteName { get; set; }

            /// <summary>
            /// Gets or sets the distance.
            /// </summary>
            /// <value>
            /// The distance away from the input point. -1 for not found.
            /// </value>
            public double Distance { get; set; }

            /// <summary>
            /// Gets or sets the milepost.
            /// </summary>
            /// <value>
            /// The closest milepost value rounded to two decimal places.
            /// </value>
            public double Milepost { get; set; }
        }
    }
}