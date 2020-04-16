using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DaqAirOilGasEmissionsPoint
    {
        public int Xid { get; set; }
        public decimal? SourceId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string CompanyNa { get; set; }
        public string SiteName { get; set; }
        public decimal? TotTpy { get; set; }
        public string MapLabel { get; set; }
        public decimal? NoxTpy { get; set; }
        public decimal? VocTpy { get; set; }
        public Point Shape { get; set; }
    }
}
