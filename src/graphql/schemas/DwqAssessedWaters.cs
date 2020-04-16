using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqAssessedWaters
    {
        public int Xid { get; set; }
        public string AuName { get; set; }
        public string AssessId { get; set; }
        public string AuDescrip { get; set; }
        public string BenClass { get; set; }
        public decimal? RiverMile { get; set; }
        public string MgmtUnit { get; set; }
        public string Assessment { get; set; }
        public string Impairment { get; set; }
        public string TmdlReqrd { get; set; }
        public string TmdlApprv { get; set; }
        public string HabImpair { get; set; }
        public string Protected { get; set; }
        public string BluRibbon { get; set; }
        public string AntiDegrd { get; set; }
        public string TmdlInfo { get; set; }
        public string Maplabel { get; set; }
        public string NewAuid { get; set; }
        public decimal? Perimeter { get; set; }
        public decimal? AreaM2 { get; set; }
        public decimal? GisAcres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
