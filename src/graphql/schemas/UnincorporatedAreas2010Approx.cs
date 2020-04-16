using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UnincorporatedAreas2010Approx
    {
        public int Xid { get; set; }
        public string Placename { get; set; }
        public string Countynbr { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
