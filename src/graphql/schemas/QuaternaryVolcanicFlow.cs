using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class QuaternaryVolcanicFlow
    {
        public int Xid { get; set; }
        public decimal? Locnum { get; set; }
        public decimal? Feature { get; set; }
        public decimal? Type { get; set; }
        public decimal? Age { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
