using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HealthSmallStatisticalAreas2017
    {
        public int Xid { get; set; }
        public decimal? AreaCode { get; set; }
        public string AreaName { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
