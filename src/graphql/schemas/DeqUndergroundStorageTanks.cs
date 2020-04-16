using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DeqUndergroundStorageTanks
    {
        public int Xid { get; set; }
        public decimal? Facilityid { get; set; }
        public string Derrid { get; set; }
        public string Locname { get; set; }
        public string Locstr { get; set; }
        public string Loccity { get; set; }
        public string Loccounty { get; set; }
        public string Locstate { get; set; }
        public string Loczip { get; set; }
        public string Facilityde { get; set; }
        public decimal? Northing { get; set; }
        public decimal? Easting { get; set; }
        public string Healthdist { get; set; }
        public string Ownername { get; set; }
        public string Owneraddress { get; set; }
        public string Ownercity { get; set; }
        public string Ownercounty { get; set; }
        public string Ownerstate { get; set; }
        public string Ownerzip { get; set; }
        public string Maplabel { get; set; }
        public string Cimid { get; set; }
        public decimal? Tank { get; set; }
        public decimal? Release { get; set; }
        public decimal? Openrelease { get; set; }
        public string Sitedesc { get; set; }
        public decimal? Tankriskaveragetest { get; set; }
        public string Utmdesc { get; set; }
        public Point Shape { get; set; }
    }
}
