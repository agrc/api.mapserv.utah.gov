using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CoalLeases
    {
        public int Xid { get; set; }
        public decimal? Sgid93EnergyCoalleasesArea { get; set; }
        public decimal? Perimeter { get; set; }
        public decimal? Owner { get; set; }
        public decimal? OwnerId { get; set; }
        public string Code { get; set; }
        public decimal? Symbol { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
