using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class NaturalGasServiceAreasApprox
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string Telephone { get; set; }
        public string Weblink { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
