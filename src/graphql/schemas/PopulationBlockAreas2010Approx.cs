using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PopulationBlockAreas2010Approx
    {
        public int Xid { get; set; }
        public string Srccenblk10 { get; set; }
        public decimal? Approxpopulation { get; set; }
        public decimal? Approxhouseholds { get; set; }
        public string Countynbr { get; set; }
        public string Category { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
