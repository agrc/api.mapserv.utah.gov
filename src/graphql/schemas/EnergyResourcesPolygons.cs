using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EnergyResourcesPolygons
    {
        public int Xid { get; set; }
        public string Code { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
