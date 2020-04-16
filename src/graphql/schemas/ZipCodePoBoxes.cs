using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ZipCodePoBoxes
    {
        public int Xid { get; set; }
        public decimal? Zip5 { get; set; }
        public string Description { get; set; }
        public decimal? Stadd { get; set; }
        public string Pre { get; set; }
        public string Street { get; set; }
        public string Suff { get; set; }
        public string Placename { get; set; }
        public string Zip { get; set; }
        public string PoboxType { get; set; }
        public Point Shape { get; set; }
    }
}
