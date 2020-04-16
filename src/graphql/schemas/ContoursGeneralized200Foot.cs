using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ContoursGeneralized200Foot
    {
        public int Xid { get; set; }
        public decimal? Contourelevation { get; set; }
        public string Elevationrange { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
