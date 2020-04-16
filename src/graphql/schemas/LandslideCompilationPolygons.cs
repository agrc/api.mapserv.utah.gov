using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LandslideCompilationPolygons
    {
        public int Xid { get; set; }
        public decimal? Statelsid { get; set; }
        public string Lsunit { get; set; }
        public string Movetype { get; set; }
        public string Historical { get; set; }
        public string Geolunit { get; set; }
        public string Mapscale { get; set; }
        public string Mapname { get; set; }
        public string Pubdate { get; set; }
        public string AuthorS { get; set; }
        public string Mapbound { get; set; }
        public string Multsource { get; set; }
        public string Affunit { get; set; }
        public string Moveunit { get; set; }
        public string Movecause { get; set; }
        public string Notes { get; set; }
        public decimal? SqMiles { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
