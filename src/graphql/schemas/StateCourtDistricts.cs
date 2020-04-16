using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StateCourtDistricts
    {
        public int Xid { get; set; }
        public string District { get; set; }
        public decimal? Color { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
