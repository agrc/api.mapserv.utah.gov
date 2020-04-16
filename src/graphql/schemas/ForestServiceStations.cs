using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ForestServiceStations
    {
        public int Xid { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public decimal? Fips { get; set; }
        public Point Shape { get; set; }
    }
}
