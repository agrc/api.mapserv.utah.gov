using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GolfCourses
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public decimal? Holes { get; set; }
        public decimal? Par { get; set; }
        public string Type { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
