using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class NwsForecastZones
    {
        public int Xid { get; set; }
        public string State { get; set; }
        public string Cwa { get; set; }
        public string TimeZone { get; set; }
        public string FeArea { get; set; }
        public string Zone { get; set; }
        public string Name { get; set; }
        public string StateZone { get; set; }
        public string Ajoin0 { get; set; }
        public string Ajoin1 { get; set; }
        public decimal? Lon { get; set; }
        public decimal? Lat { get; set; }
        public string Shortname { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
