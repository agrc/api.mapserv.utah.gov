using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PostOffices
    {
        public int Xid { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal? Zip { get; set; }
        public string Type { get; set; }
        public Point Shape { get; set; }
    }
}
