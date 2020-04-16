using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TransitSpecialTaxAreas
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public decimal? Entitynbr { get; set; }
        public decimal? Entityyr { get; set; }
        public string Shortdesc { get; set; }
        public string Sdekey { get; set; }
        public decimal? Taxdist { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
