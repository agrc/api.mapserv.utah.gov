using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class QuaternaryFaults
    {
        public int Xid { get; set; }
        public string Faultnum { get; set; }
        public string Faultzone { get; set; }
        public string Faultname { get; set; }
        public string Sectionname { get; set; }
        public string Strandname { get; set; }
        public string Mappedscale { get; set; }
        public string Dipdirection { get; set; }
        public string Slipsense { get; set; }
        public string Sliprate { get; set; }
        public string Mappingconstraint { get; set; }
        public string Faultclass { get; set; }
        public string Faultage { get; set; }
        public string Label { get; set; }
        public string Summary { get; set; }
        public string UsgsLink { get; set; }
        public string Notes { get; set; }
        public decimal? Ruleid { get; set; }
        public byte[] Override { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
