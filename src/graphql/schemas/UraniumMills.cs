using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UraniumMills
    {
        public int Xid { get; set; }
        public string MillId { get; set; }
        public string Name { get; set; }
        public string Operator { get; set; }
        public string Township { get; set; }
        public string Range { get; set; }
        public decimal? Section { get; set; }
        public decimal? UtmN { get; set; }
        public decimal? UtmE { get; set; }
        public string County { get; set; }
        public Point Shape { get; set; }
    }
}
