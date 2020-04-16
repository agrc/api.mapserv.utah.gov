using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class DeqmapEiImpactedMedia
    {
        public int Xid { get; set; }
        public decimal? Derrid { get; set; }
        public string Waterwayname { get; set; }
        public string Distancetosurfacewater { get; set; }
        public string Impactedmediaother { get; set; }
        public string Impactedmedia { get; set; }
        public decimal? InNearwater { get; set; }
        public string Description { get; set; }
        public string Landuse { get; set; }
        public decimal? Id { get; set; }
    }
}
