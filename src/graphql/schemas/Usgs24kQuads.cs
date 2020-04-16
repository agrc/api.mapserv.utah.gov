using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Usgs24kQuads
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string OhioCode { get; set; }
        public string Tile { get; set; }
        public string Path { get; set; }
        public string Ext { get; set; }
        public decimal? Size { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
