using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Usgs24kQuarterQuads
    {
        public int Xid { get; set; }
        public string TileName { get; set; }
        public string Name { get; set; }
        public string OhioCode { get; set; }
        public string Quadrant { get; set; }
        public string TileCoord { get; set; }
        public string Ohio2 { get; set; }
        public string OhioCoord { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
