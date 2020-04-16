using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BlmDistricts
    {
        public int Xid { get; set; }
        public string FoName { get; set; }
        public string FoCode { get; set; }
        public string DoName { get; set; }
        public string DoCode { get; set; }
        public decimal? ShapeLeng { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
