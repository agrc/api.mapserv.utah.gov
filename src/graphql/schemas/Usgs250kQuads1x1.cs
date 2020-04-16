using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Usgs250kQuads1x1
    {
        public int Xid { get; set; }
        public string TileName { get; set; }
        public string Location { get; set; }
        public string UsgsCode { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
