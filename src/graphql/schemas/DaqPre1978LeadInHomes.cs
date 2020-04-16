using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DaqPre1978LeadInHomes
    {
        public int Xid { get; set; }
        public string ParcelId { get; set; }
        public string Fulladd { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Addnum { get; set; }
        public string PropClass { get; set; }
        public decimal? BuiltYr { get; set; }
        public DateTime? CurrentAsof { get; set; }
        public string County { get; set; }
        public Point Shape { get; set; }
    }
}
