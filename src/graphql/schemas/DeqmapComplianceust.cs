using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapComplianceust
    {
        public int Xid { get; set; }
        public decimal? Facilityid { get; set; }
        public DateTime? Compdate { get; set; }
        public string Scientist { get; set; }
        public string Citation { get; set; }
        public string Description { get; set; }
    }
}
