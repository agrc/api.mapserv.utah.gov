using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class SchoolBoardDistrictsMembers
    {
        public int Xid { get; set; }
        public decimal? Dist { get; set; }
        public decimal? Color4 { get; set; }
        public string Board { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
