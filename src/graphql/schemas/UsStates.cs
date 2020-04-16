using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UsStates
    {
        public int Xid { get; set; }
        public string StateName { get; set; }
        public string StateFips { get; set; }
        public string StateAbbr { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
