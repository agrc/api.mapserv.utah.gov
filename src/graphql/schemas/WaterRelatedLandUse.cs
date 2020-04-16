using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class WaterRelatedLandUse
    {
        public int Xid { get; set; }
        public decimal? Luid { get; set; }
        public string Landuse { get; set; }
        public string Cropgroup { get; set; }
        public string Description { get; set; }
        public string IrrMethod { get; set; }
        public decimal? Acres { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Basin { get; set; }
        public string Subarea { get; set; }
        public string LabelClass { get; set; }
        public string Label { get; set; }
        public string ClassName { get; set; }
        public string Oldlanduse { get; set; }
        public string LuGroup { get; set; }
        public string SurvYear { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
