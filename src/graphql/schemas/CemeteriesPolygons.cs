using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CemeteriesPolygons
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
