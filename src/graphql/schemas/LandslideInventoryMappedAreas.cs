using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LandslideInventoryMappedAreas
    {
        public int Xid { get; set; }
        public decimal? AreaSqmi { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
