using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CountyCourtsOfUtah
    {
        public int Xid { get; set; }
        public string Court { get; set; }
        public decimal? Color { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
