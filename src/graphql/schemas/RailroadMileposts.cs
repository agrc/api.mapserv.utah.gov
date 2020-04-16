using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class RailroadMileposts
    {
        public int Xid { get; set; }
        public string Division { get; set; }
        public string RrMilepos { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public string Label { get; set; }
        public Point Shape { get; set; }
    }
}
