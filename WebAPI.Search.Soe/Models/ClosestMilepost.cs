namespace WebAPI.Search.Soe.Models
{
    public class ClosestMilepost
    {
        public ClosestMilepost()
        {
            Distance = double.MaxValue;
        }

        /// <summary>
        /// Gets or sets the milepost.
        /// </summary>
        /// <value>
        /// The milepost.
        /// </value>
        public double Milepost { get; set; }

        /// <summary>
        /// Gets or sets the route name from the lrs data.
        /// </summary>
        /// <value>
        /// The route. RT_NAME
        /// </value>
        public string Route { get; set; }

        /// <summary>
        /// Gets or sets the distance.
        /// </summary>
        /// <value>
        /// The distance in meters away from the input point.
        /// </value>
        public double Distance { get; set; }

        public bool Increasing { get; set; }

        public ClosestMilepost(double milepost, string route, double distance, bool increasing)
        {
            Milepost = milepost;
            Route = route;
            Distance = distance;
            Increasing = increasing;
        }
    }
}