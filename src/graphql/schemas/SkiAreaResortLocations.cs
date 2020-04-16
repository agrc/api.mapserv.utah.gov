using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SkiAreaResortLocations
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public Point Shape { get; set; }
    }
}
