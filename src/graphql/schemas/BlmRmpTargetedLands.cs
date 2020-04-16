using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BlmRmpTargetedLands
    {
        public int Xid { get; set; }
        public string Type { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
