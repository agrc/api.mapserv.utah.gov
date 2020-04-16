using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UdotMapScenicByways
    {
        public int Xid { get; set; }
        public decimal? Symbol { get; set; }
        public string Name { get; set; }
        public DateTime? AdoptedDa { get; set; }
        public string RoadLeade { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
