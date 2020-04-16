using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HistoricDistricts
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public decimal? Zip { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
