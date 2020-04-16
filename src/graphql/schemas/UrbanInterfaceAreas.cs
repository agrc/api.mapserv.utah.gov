using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrbanInterfaceAreas
    {
        public int Xid { get; set; }
        public string Type { get; set; }
        public decimal? Fuel { get; set; }
        public decimal? Lotsize { get; set; }
        public string Name { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
