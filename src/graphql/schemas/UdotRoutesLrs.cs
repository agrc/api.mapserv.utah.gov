using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UdotRoutesLrs
    {
        public int Xid { get; set; }
        public string Label { get; set; }
        public string RtName { get; set; }
        public string RtDir { get; set; }
        public string RtType { get; set; }
        public string RtInt { get; set; }
        public string RtRampnbr { get; set; }
        public decimal? RtDirId { get; set; }
        public string RtMindesc { get; set; }
        public DateTime? EffDate { get; set; }
        public DateTime? DepDate { get; set; }
        public string EffNotes { get; set; }
        public string DepNotes { get; set; }
        public string Carto { get; set; }
        public DateTime? RtDateB { get; set; }
        public DateTime? RtDateE { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
