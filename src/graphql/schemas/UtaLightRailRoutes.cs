using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaLightRailRoutes
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public string Direction { get; set; }
        public string Line { get; set; }
        public string Route { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
