using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AquiferBasinFillBoundary
    {
        public int Xid { get; set; }
        public int? Objectid { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
