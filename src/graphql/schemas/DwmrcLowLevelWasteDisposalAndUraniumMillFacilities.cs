using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwmrcLowLevelWasteDisposalAndUraniumMillFacilities
    {
        public int Xid { get; set; }
        public string FacilityNumber { get; set; }
        public string FacilityName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Point Shape { get; set; }
    }
}
