using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GreatSaltLakeShorelineFlooding
    {
        public int Xid { get; set; }
        public decimal? Symbol { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
