using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaCommuterRailRoutes
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
