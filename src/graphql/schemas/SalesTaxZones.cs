using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SalesTaxZones
    {
        public int Xid { get; set; }
        public string Taxdist { get; set; }
        public string Fips { get; set; }
        public string Shortdesc { get; set; }
        public string Metroname { get; set; }
        public string Special { get; set; }
        public decimal? Currrate { get; set; }
        public string Ratedate { get; set; }
        public string SstPod { get; set; }
        public string Countynbr { get; set; }
        public string Coname { get; set; }
        public string Fipsconum { get; set; }
        public string Sdekey { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
