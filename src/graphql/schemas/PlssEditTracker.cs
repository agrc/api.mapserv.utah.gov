using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PlssEditTracker
    {
        public int Xid { get; set; }
        public string Plssid { get; set; }
        public string Section { get; set; }
        public string Label { get; set; }
        public decimal? Edits { get; set; }
        public string Name { get; set; }
        public DateTime? EditDate { get; set; }
        public string Notes { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
