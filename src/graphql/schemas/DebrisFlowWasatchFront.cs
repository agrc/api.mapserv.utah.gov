using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DebrisFlowWasatchFront
    {
        public int Xid { get; set; }
        public string Hazard { get; set; }
        public string Requirement { get; set; }
        public string Relativehazard { get; set; }
        public string Hazcode { get; set; }
        public string Comments { get; set; }
        public string County { get; set; }
        public string Source { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
