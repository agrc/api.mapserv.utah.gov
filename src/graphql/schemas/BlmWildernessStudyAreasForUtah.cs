using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BlmWildernessStudyAreasForUtah
    {
        public int Xid { get; set; }
        public decimal? WiwsaUt24 { get; set; }
        public decimal? WiwsaUt1 { get; set; }
        public string WsaNumber { get; set; }
        public decimal? WsaSuitab { get; set; }
        public string WsaName { get; set; }
        public DateTime? WsaDate { get; set; }
        public decimal? WsaAuth { get; set; }
        public decimal? WsaAcres { get; set; }
        public string WsaType { get; set; }
        public string WsaValues { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
