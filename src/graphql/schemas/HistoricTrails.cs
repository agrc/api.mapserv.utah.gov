using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HistoricTrails
    {
        public int Xid { get; set; }
        public string Trailname { get; set; }
        public string Routename { get; set; }
        public string Altroutename { get; set; }
        public string County { get; set; }
        public string State { get; set; }
        public string Scale { get; set; }
        public string Adminagency { get; set; }
        public string Source { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
