using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapCauselust
    {
        public int Xid { get; set; }
        public decimal? Causekey { get; set; }
        public string Causereal { get; set; }
        public string Subrelease { get; set; }
        public string Methdet { get; set; }
        public decimal? Lustkey { get; set; }
    }
}
