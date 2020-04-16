using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ArchaeologySites
    {
        public int Xid { get; set; }
        public string Presenceyn { get; set; }
        public string Warning1 { get; set; }
        public string Warning2 { get; set; }
        public string Warning3 { get; set; }
        public string Warning4 { get; set; }
        public string AddInfo { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
