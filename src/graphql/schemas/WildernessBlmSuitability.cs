using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessBlmSuitability
    {
        public int Xid { get; set; }
        public string Unit { get; set; }
        public string Blmname { get; set; }
        public string IsThisSt { get; set; }
        public decimal? Suitabilit { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
