using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GeologicDikes
    {
        public int Xid { get; set; }
        public string LType { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
