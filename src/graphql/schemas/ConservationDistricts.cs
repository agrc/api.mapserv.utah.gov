using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ConservationDistricts
    {
        public int Xid { get; set; }
        public string Distname { get; set; }
        public decimal? Distno { get; set; }
        public decimal? Zoneno { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
