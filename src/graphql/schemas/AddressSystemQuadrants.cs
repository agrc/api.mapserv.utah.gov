using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AddressSystemQuadrants
    {
        public int Xid { get; set; }
        public string GridName { get; set; }
        public string Quadrant { get; set; }
        public string County { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
