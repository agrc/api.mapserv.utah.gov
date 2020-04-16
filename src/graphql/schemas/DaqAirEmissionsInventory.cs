using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DaqAirEmissionsInventory
    {
        public int Xid { get; set; }
        public decimal? DaqId { get; set; }
        public string Company { get; set; }
        public string MapLabel { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public decimal? Year { get; set; }
        public decimal? CarbonMonoxide { get; set; }
        public decimal? OxidesNitrogen { get; set; }
        public decimal? Ammonia { get; set; }
        public decimal? ParticulateMatterPm10 { get; set; }
        public decimal? OxidesSulfur { get; set; }
        public decimal? Pm25 { get; set; }
        public decimal? VolatileOrganicCompounds { get; set; }
        public decimal? Easting { get; set; }
        public decimal? Northing { get; set; }
        public Point Shape { get; set; }
    }
}
