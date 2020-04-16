using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HealthSmallStatisticalAreasObesityAndActivity
    {
        public int Xid { get; set; }
        public string Smallareanumber { get; set; }
        public string Smallareaname { get; set; }
        public decimal? Hiindex { get; set; }
        public decimal? Population2017 { get; set; }
        public decimal? PopdensitySqmi { get; set; }
        public decimal? PhysactivePercent { get; set; }
        public decimal? ObesityPercent { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
