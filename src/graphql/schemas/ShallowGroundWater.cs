using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ShallowGroundWater
    {
        public int Xid { get; set; }
        public decimal? Depth { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
