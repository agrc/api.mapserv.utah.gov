using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SemiPrimitivePropEmeryCounty
    {
        public int Xid { get; set; }
        public string Unit { get; set; }
        public string IsThisState { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
