using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WildfirePerimeters
    {
        public int Xid { get; set; }
        public string UnitId { get; set; }
        public string FireNum { get; set; }
        public string FireName { get; set; }
        public DateTime? Date { get; set; }
        public string Time { get; set; }
        public string Comments { get; set; }
        public string Agency { get; set; }
        public string Active { get; set; }
        public string Fire { get; set; }
        public string Year { get; set; }
        public DateTime? LoadDate { get; set; }
        public string InciwebId { get; set; }
        public decimal? Acres { get; set; }
        public string Source { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
