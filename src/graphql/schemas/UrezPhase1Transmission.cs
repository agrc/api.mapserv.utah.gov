using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase1Transmission
    {
        public int Xid { get; set; }
        public string Kv { get; set; }
        public decimal? Color { get; set; }
        public string Comment { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
