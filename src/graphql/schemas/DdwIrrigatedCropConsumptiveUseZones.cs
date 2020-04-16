using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DdwIrrigatedCropConsumptiveUseZones
    {
        public int Xid { get; set; }
        public string Cuzone { get; set; }
        public string Cuzonedesc { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
