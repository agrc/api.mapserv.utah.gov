using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LiquefactionPotential
    {
        public int Xid { get; set; }
        public decimal? Pcode { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
