using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HighestPeaks
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Elevation { get; set; }
        public string QuadName { get; set; }
        public decimal? Rank { get; set; }
        public string Combined { get; set; }
        public string County { get; set; }
        public Point Shape { get; set; }
    }
}
