using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StreamMonitorSites
    {
        public int Xid { get; set; }
        public decimal? Station { get; set; }
        public decimal? Active { get; set; }
        public Point Shape { get; set; }
    }
}
