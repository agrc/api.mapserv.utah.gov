using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapSoiltype
    {
        public int Xid { get; set; }
        public decimal? Lustkey { get; set; }
        public string Soil { get; set; }
        public string Soilcom { get; set; }
        public decimal? Soiltypekey { get; set; }
    }
}
