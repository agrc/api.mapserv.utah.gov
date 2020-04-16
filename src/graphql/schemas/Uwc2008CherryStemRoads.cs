using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Uwc2008CherryStemRoads
    {
        public int Xid { get; set; }
        public string PropType { get; set; }
        public decimal? Id { get; set; }
        public decimal? ShapeLeng { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
