using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AvalanchePaths
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string SizeOfSlidepath { get; set; }
        public string ReturnInterval { get; set; }
        public string StartingZoneElev { get; set; }
        public string VerticalFall { get; set; }
        public string DistanceToHighway { get; set; }
        public string StartingZoneAspect { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
