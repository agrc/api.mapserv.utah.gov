using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TaxAreas2018
    {
        public int Xid { get; set; }
        public decimal? TxareaYr { get; set; }
        public string TxareaCo { get; set; }
        public string TxareaId { get; set; }
        public string TxareaSub { get; set; }
        public decimal? ComboId { get; set; }
        public decimal? ComboInst { get; set; }
        public decimal? TxareaTemp { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
