using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TurnGpsBaselines
    {
        public int Xid { get; set; }
        public decimal? Miles { get; set; }
        public decimal? Kilometers { get; set; }
        public string Status { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
