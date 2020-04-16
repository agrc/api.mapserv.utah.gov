using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GrazingAllotments
    {
        public int Xid { get; set; }
        public string Manager { get; set; }
        public string Allotname { get; set; }
        public string Pasturenm { get; set; }
        public string Allotno { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
