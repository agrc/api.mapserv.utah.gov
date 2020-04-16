using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SkiTrailsXc
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string XcArea { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
