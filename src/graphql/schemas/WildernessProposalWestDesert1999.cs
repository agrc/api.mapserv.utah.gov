using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessProposalWestDesert1999
    {
        public int Xid { get; set; }
        public string Data { get; set; }
        public string IsThisState { get; set; }
        public string Unit { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
