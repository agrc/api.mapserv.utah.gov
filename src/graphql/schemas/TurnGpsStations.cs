using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TurnGpsStations
    {
        public int Xid { get; set; }
        public string StationName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string EllipsoidHeight { get; set; }
        public decimal? LatDd { get; set; }
        public decimal? LongDd { get; set; }
        public string Status { get; set; }
        public string TypeOfStation { get; set; }
        public string Location { get; set; }
        public string Stationcode { get; set; }
        public string Antennatype { get; set; }
        public Point Shape { get; set; }
    }
}
