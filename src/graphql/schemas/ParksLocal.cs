using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ParksLocal
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public decimal? Acres { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
