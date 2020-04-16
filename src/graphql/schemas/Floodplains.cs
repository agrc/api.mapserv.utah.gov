using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Floodplains
    {
        public int Xid { get; set; }
        public string DfirmId { get; set; }
        public string VersionId { get; set; }
        public string FldArId { get; set; }
        public string StudyTyp { get; set; }
        public string FldZone { get; set; }
        public string ZoneSubty { get; set; }
        public string Floodway { get; set; }
        public string SfhaTf { get; set; }
        public decimal? StaticBfe { get; set; }
        public string VDatum { get; set; }
        public decimal? Depth { get; set; }
        public string LenUnit { get; set; }
        public decimal? Velocity { get; set; }
        public string VelUnit { get; set; }
        public string ArRevert { get; set; }
        public string ArSubtrv { get; set; }
        public decimal? BfeRevert { get; set; }
        public decimal? DepRevert { get; set; }
        public string DualZone { get; set; }
        public string SourceCit { get; set; }
        public string Countyid { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
