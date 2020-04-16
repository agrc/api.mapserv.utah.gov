using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Contours500Foot
    {
        public int Xid { get; set; }
        public decimal? Elev { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
