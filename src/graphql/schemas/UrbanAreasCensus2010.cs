using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrbanAreasCensus2010
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal? Population { get; set; }
        public decimal? HousingUnits { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
