using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LandslideInventoryPolygons
    {
        public int Xid { get; set; }
        public string Category { get; set; }
        public string DMaterial { get; set; }
        public string DMoveType { get; set; }
        public string DName { get; set; }
        public string DThickness { get; set; }
        public decimal? DMoveDir { get; set; }
        public string DLandform { get; set; }
        public string SName { get; set; }
        public string Activity { get; set; }
        public string Confidence { get; set; }
        public string Mapper { get; set; }
        public string PeerRev { get; set; }
        public string Comments { get; set; }
        public decimal? DHMove1 { get; set; }
        public decimal? DHMove2 { get; set; }
        public decimal? DHMove3 { get; set; }
        public string DGeologicUnit1 { get; set; }
        public string DGeologicUnit2 { get; set; }
        public string Publication { get; set; }
        public string MappedScale { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
