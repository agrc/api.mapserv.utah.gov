using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase1SolarZones
    {
        public int Xid { get; set; }
        public decimal? Dnr { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
