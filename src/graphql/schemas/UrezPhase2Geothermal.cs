using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase2Geothermal
    {
        public int Xid { get; set; }
        public string Project { get; set; }
        public decimal? Mw { get; set; }
        public decimal? Gwh { get; set; }
        public string UrezName { get; set; }
        public decimal? DistanceM { get; set; }
        public string Subid { get; set; }
        public Point Shape { get; set; }
    }
}
