using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UdotTenthMileRefPoints
    {
        public int Xid { get; set; }
        public string Label { get; set; }
        public string RtName { get; set; }
        public decimal? Mp { get; set; }
        public decimal? Xcoord { get; set; }
        public decimal? Ycoord { get; set; }
        public string RtType { get; set; }
        public string RtDir { get; set; }
        public string Carto { get; set; }
        public byte[] SeAnnoCadData { get; set; }
        public Point Shape { get; set; }
    }
}
