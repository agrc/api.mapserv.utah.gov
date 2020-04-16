using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HealthSmallStatisticalAreas2018
    {
        public int Xid { get; set; }
        public string Smallareanumber { get; set; }
        public string Smallareaname { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
