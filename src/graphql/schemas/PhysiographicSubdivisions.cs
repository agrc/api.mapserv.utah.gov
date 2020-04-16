using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PhysiographicSubdivisions
    {
        public int Xid { get; set; }
        public string Subdivision { get; set; }
        public string Section { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
