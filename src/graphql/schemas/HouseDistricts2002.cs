using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HouseDistricts2002
    {
        public int Xid { get; set; }
        public decimal? Dist { get; set; }
        public decimal? Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
