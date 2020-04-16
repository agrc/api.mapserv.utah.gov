using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PopulationPlacePoints2010Approx
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string County { get; set; }
        public decimal? Approxpopulation { get; set; }
        public string Incorporated { get; set; }
        public string Countyseat { get; set; }
        public string Countyseatlabel { get; set; }
        public Point Shape { get; set; }
    }
}
