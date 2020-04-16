using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UnpopulatedAreas2010Approx
    {
        public int Xid { get; set; }
        public string State { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
