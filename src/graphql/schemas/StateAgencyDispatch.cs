using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StateAgencyDispatch
    {
        public int Xid { get; set; }
        public decimal? Color { get; set; }
        public string Dispatch { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
