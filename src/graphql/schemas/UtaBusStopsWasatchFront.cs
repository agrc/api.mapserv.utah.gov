using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaBusStopsWasatchFront
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public decimal? Zipcode { get; set; }
        public string County { get; set; }
        public string Stopabbr { get; set; }
        public decimal? Y { get; set; }
        public decimal? X { get; set; }
        public Point Shape { get; set; }
    }
}
