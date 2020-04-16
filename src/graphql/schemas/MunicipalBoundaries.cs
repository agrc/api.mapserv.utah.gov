using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MunicipalBoundaries
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public string Name { get; set; }
        public decimal? Countyseat { get; set; }
        public string Shortdesc { get; set; }
        public DateTime? Updated { get; set; }
        public string Fips { get; set; }
        public decimal? Entitynbr { get; set; }
        public string Salestaxid { get; set; }
        public decimal? Imscolor { get; set; }
        public string Minname { get; set; }
        public decimal? Poplastcensus { get; set; }
        public decimal? Poplastestimate { get; set; }
        public string Gnis { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
