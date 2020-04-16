using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ForestServiceBoundaries
    {
        public int Xid { get; set; }
        public decimal? ForestNum { get; set; }
        public string ForestName { get; set; }
        public string AdminForest { get; set; }
        public string Forest { get; set; }
        public string MapName { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
