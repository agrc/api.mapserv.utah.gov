using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Usgs3dep1kGrid
    {
        public int Xid { get; set; }
        public string StateName { get; set; }
        public string CountyName { get; set; }
        public string TileId { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
