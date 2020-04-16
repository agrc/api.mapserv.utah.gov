using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Wetlands
    {
        public int Xid { get; set; }
        public string Attribute { get; set; }
        public string WetlandType { get; set; }
        public decimal? Acres { get; set; }
        public string System { get; set; }
        public string Class { get; set; }
        public string Regime { get; set; }
        public string Modifier { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
