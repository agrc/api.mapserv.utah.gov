using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessProposalUwc1995
    {
        public int Xid { get; set; }
        public string Unit { get; set; }
        public decimal? Acres { get; set; }
        public string IsThisState { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
