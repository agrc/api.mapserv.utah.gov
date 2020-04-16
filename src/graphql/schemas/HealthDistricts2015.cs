using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HealthDistricts2015
    {
        public int Xid { get; set; }
        public string Distname { get; set; }
        public decimal? IdNum { get; set; }
        public decimal? Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
