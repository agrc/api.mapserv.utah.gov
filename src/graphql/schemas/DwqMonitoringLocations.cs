using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqMonitoringLocations
    {
        public int Xid { get; set; }
        public decimal? Rec { get; set; }
        public string MonlocId { get; set; }
        public decimal? MonlocIds { get; set; }
        public string AwqmsType { get; set; }
        public string BenClass { get; set; }
        public string Name { get; set; }
        public string Hydrologic { get; set; }
        public string County { get; set; }
        public string Unit { get; set; }
        public decimal? Nad83utmX { get; set; }
        public decimal? Nad83utmY { get; set; }
        public decimal? Lonnad83dd { get; set; }
        public decimal? Latnad83dd { get; set; }
        public string AuName { get; set; }
        public string AuAssess { get; set; }
        public string State { get; set; }
        public string SiteSpec { get; set; }
        public decimal? Standard { get; set; }
        public string Units { get; set; }
        public string Owner { get; set; }
        public string AntiDeg { get; set; }
        public string Maplabel { get; set; }
        public Point Shape { get; set; }
    }
}
