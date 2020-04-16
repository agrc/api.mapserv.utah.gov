using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class RuralTelecomBoundaries
    {
        public int Xid { get; set; }
        public decimal? TeleBound { get; set; }
        public string Provider { get; set; }
        public string Layer { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Weblink { get; set; }
        public decimal? Color4 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
