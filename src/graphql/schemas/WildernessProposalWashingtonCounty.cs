using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessProposalWashingtonCounty
    {
        public int Xid { get; set; }
        public string Unit { get; set; }
        public decimal? Acres { get; set; }
        public string Blmname { get; set; }
        public string UnitLabel { get; set; }
        public string IsThisSt { get; set; }
        public decimal? ShapeLeng { get; set; }
        public string Label { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
