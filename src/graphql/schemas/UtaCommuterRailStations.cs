using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaCommuterRailStations
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public string Address { get; set; }
        public string Parknride { get; set; }
        public string Stationname { get; set; }
        public string Status { get; set; }
        public string Utastopid { get; set; }
        public string Linename { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public Point Shape { get; set; }
    }
}
