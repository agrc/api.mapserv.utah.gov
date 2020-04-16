using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ConservationZones
    {
        public int Xid { get; set; }
        public decimal? Zoneno { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
