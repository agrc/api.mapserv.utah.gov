using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwmrcSolidWasteFacilities
    {
        public int Xid { get; set; }
        public string FacilityName { get; set; }
        public string FacilityNumber { get; set; }
        public string StreetAddress { get; set; }
        public string CityName { get; set; }
        public decimal? CityId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Point Shape { get; set; }
    }
}
