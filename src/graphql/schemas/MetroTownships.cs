using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MetroTownships
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public string Name { get; set; }
        public string Shortdesc { get; set; }
        public DateTime? Updated { get; set; }
        public string Fips { get; set; }
        public string Minname { get; set; }
        public decimal? Poplastcensus { get; set; }
        public decimal? Poplastestimate { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
