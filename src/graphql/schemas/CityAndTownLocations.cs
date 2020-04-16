using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CityAndTownLocations
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string County { get; set; }
        public string Countynbr { get; set; }
        public decimal? Countyseat { get; set; }
        public decimal? Population { get; set; }
        public string Type { get; set; }
        public string Carto { get; set; }
        public Point Shape { get; set; }
    }
}
