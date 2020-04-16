using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CoalMinesUgs
    {
        public int Xid { get; set; }
        public string Mineid { get; set; }
        public string Type { get; set; }
        public string County { get; set; }
        public decimal? Num { get; set; }
        public string MineStat { get; set; }
        public string PermStat { get; set; }
        public string Msu { get; set; }
        public string Name { get; set; }
        public string MapName { get; set; }
        public string Minerals { get; set; }
        public string IndusSeg { get; set; }
        public decimal? Utmx { get; set; }
        public decimal? Utmy { get; set; }
        public string Operator { get; set; }
        public Point Shape { get; set; }
    }
}
