using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SkiLifts
    {
        public int Xid { get; set; }
        public string Resort { get; set; }
        public string LiftName { get; set; }
        public string Type { get; set; }
        public decimal? Capacity { get; set; }
        public decimal? BaseElevation { get; set; }
        public decimal? TopElevation { get; set; }
        public string County { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
