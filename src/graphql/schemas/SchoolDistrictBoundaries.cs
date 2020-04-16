using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SchoolDistrictBoundaries
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public decimal? Totenroll { get; set; }
        public string Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
