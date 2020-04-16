using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GrazingImprovementRegions
    {
        public int Xid { get; set; }
        public string Gipregion { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
