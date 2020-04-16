using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DebrisFlowWasatchStudyBoundary
    {
        public int Xid { get; set; }
        public string Author { get; set; }
        public decimal? Year { get; set; }
        public string Pubagency { get; set; }
        public string Publication { get; set; }
        public string Pubnumber { get; set; }
        public string Hazard { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
