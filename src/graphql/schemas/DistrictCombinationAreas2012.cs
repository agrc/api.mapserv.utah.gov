using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DistrictCombinationAreas2012
    {
        public int Xid { get; set; }
        public string Comboid { get; set; }
        public string Congress { get; set; }
        public string Senate { get; set; }
        public string House { get; set; }
        public string School { get; set; }
        public string Color4senate { get; set; }
        public string Color4house { get; set; }
        public string Color4school { get; set; }
        public string County { get; set; }
        public decimal? Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
