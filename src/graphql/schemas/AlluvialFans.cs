using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AlluvialFans
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public string Watercourse { get; set; }
        public string GeologicMapDescription { get; set; }
        public string RelativeHazard { get; set; }
        public string Municipality { get; set; }
        public string County { get; set; }
        public string FemaFloodZone { get; set; }
        public string GeologicMapUnit { get; set; }
        public string UgsFloodHazard { get; set; }
        public string FieldNotes { get; set; }
        public string UgsLandslideDescription { get; set; }
        public string UgsLandslideMoveType { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
