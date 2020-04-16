using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HealthCareDistricts
    {
        public int Xid { get; set; }
        public string Distname { get; set; }
        public decimal? IdNum { get; set; }
        public decimal? Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
