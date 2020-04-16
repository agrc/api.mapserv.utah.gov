using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildernessBlm98Reinventory
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Unit { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
