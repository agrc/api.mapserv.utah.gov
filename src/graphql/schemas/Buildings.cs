using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Buildings
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip5 { get; set; }
        public string County { get; set; }
        public decimal? Fips { get; set; }
        public string ParcelId { get; set; }
        public string SrcYear { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
