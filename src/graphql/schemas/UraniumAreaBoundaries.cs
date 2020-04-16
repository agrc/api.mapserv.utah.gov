using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UraniumAreaBoundaries
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public string Name { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
