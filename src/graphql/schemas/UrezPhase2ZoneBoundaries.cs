using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase2ZoneBoundaries
    {
        public int Xid { get; set; }
        public string Altname { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
