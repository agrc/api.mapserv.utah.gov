using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessProposalHr1745
    {
        public int Xid { get; set; }
        public string Unit { get; set; }
        public string IsThisState { get; set; }
        public string UnitLabel { get; set; }
        public decimal? Acres { get; set; }
        public string Blmname { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
