using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MunicipalBoundariesCartography
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
