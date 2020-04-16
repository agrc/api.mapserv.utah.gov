using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase1WindDrainage
    {
        public int Xid { get; set; }
        public string Sitename { get; set; }
        public Point Shape { get; set; }
    }
}
