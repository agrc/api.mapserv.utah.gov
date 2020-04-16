using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GreatSaltLakeFlooding
    {
        public int Xid { get; set; }
        public string Zone { get; set; }
        public decimal? Symbol { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
