using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LandslideCompilationScarps
    {
        public int Xid { get; set; }
        public string Lsunit { get; set; }
        public string Historical { get; set; }
        public string Mapscale { get; set; }
        public string Mapname { get; set; }
        public string Pubdate { get; set; }
        public string AuthorS { get; set; }
        public string Mapbound { get; set; }
        public string Notes { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
