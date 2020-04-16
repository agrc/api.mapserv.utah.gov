using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AvalancheRoseTriCanyons
    {
        public int Xid { get; set; }
        public decimal? Gridcode { get; set; }
        public decimal? Aspect { get; set; }
        public decimal? Elevation { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
