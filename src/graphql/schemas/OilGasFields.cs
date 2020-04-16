using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class OilGasFields
    {
        public int Xid { get; set; }
        public decimal? Fieldnum { get; set; }
        public string Fieldname { get; set; }
        public string Status { get; set; }
        public string County { get; set; }
        public string Date { get; set; }
        public string ProdForm { get; set; }
        public string From2 { get; set; }
        public string Form3 { get; set; }
        public string Form4 { get; set; }
        public string Comments { get; set; }
        public string DiscWell { get; set; }
        public DateTime? Loaddate { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
