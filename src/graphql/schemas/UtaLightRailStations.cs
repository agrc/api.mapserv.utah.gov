using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaLightRailStations
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public string Address { get; set; }
        public string Parknride { get; set; }
        public string Stationname { get; set; }
        public string Linename { get; set; }
        public string Status { get; set; }
        public Point Shape { get; set; }
    }
}
