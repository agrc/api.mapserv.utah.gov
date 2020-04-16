using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessProposalUwc2008
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public decimal? UwcProp { get; set; }
        public string Region { get; set; }
        public decimal? DateRev { get; set; }
        public string Cluster { get; set; }
        public decimal? Polygonid { get; set; }
        public decimal? Scale { get; set; }
        public decimal? Angle { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
