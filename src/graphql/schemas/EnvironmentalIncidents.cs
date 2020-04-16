using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EnvironmentalIncidents
    {
        public int Xid { get; set; }
        public decimal? Derrid { get; set; }
        public string Sitedesc { get; set; }
        public decimal? Northing { get; set; }
        public decimal? Easting { get; set; }
        public string TitleEventname { get; set; }
        public string AddressLocation { get; set; }
        public string NearestCity { get; set; }
        public string DateDiscovered { get; set; }
        public string ResponsibleParty { get; set; }
        public string County { get; set; }
        public string MapLabel { get; set; }
        public string IncidentSummary { get; set; }
        public DateTime? DateDiscoveredForFilter { get; set; }
        public Point Shape { get; set; }
    }
}
