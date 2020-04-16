using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HexagonServiceDates
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public decimal? Year { get; set; }
        public string Resolution { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
