using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DaqAirMonitorByStation
    {
        public int Xid { get; set; }
        public string Stationid { get; set; }
        public string Stationname { get; set; }
        public string Maplabel { get; set; }
        public string Epaid { get; set; }
        public string Stationaddress { get; set; }
        public string Stationcity { get; set; }
        public string Stationcounty { get; set; }
        public string Stationzip { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public Point Shape { get; set; }
    }
}
