using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LiquorStores
    {
        public int Xid { get; set; }
        public decimal? Storenumber { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public decimal? Zip { get; set; }
        public string Phone { get; set; }
        public string Onlystore { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public Point Shape { get; set; }
    }
}
