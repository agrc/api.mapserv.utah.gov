using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqAssessedLakes
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public decimal? HucUnit { get; set; }
        public string LowCase { get; set; }
        public string MgmtUnit { get; set; }
        public string WbodyId { get; set; }
        public decimal? Acre { get; set; }
        public string AcreSourc { get; set; }
        public decimal? Acres { get; set; }
        public string County { get; set; }
        public decimal? Nad83utmX { get; set; }
        public decimal? Nad83utmY { get; set; }
        public decimal? LongNad83 { get; set; }
        public decimal? LatNad83 { get; set; }
        public string Maplabel { get; set; }
        public Point Shape { get; set; }
    }
}
