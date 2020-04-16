using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DaqAirOilGasEmissions
    {
        public int Xid { get; set; }
        public decimal? CellId { get; set; }
        public decimal? Noxtpy { get; set; }
        public decimal? Voctpy { get; set; }
        public decimal? Noxvoctpy { get; set; }
        public string Cellid1 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
