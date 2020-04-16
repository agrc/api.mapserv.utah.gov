using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UniqueHouseSenate2002
    {
        public int Xid { get; set; }
        public string Combine { get; set; }
        public string Senlbl { get; set; }
        public decimal? Sendist { get; set; }
        public string Replbl { get; set; }
        public decimal? Repdist { get; set; }
        public decimal? Senclr4 { get; set; }
        public decimal? Repclr4 { get; set; }
        public decimal? Color6 { get; set; }
        public decimal? Ushouse { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
