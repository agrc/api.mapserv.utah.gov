using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StateBoundary
    {
        public int Xid { get; set; }
        public string State { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
