using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase1GeothermalZones
    {
        public int Xid { get; set; }
        public string Areaname { get; set; }
        public string Comments { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
