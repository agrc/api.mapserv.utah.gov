using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Stations
    {
        public int Xid { get; set; }
        public string StationId { get; set; }
        public string Organizati { get; set; }
        public string StationNa { get; set; }
        public string PrimaryTy { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Hydrologic { get; set; }
        public decimal? Inutah { get; set; }
        public Point Shape { get; set; }
    }
}
