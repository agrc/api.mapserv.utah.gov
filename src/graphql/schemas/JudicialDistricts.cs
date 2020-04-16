using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class JudicialDistricts
    {
        public int Xid { get; set; }
        public string District { get; set; }
        public string DistrictCourt { get; set; }
        public string JuvenileCourt { get; set; }
        public string Counties { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
