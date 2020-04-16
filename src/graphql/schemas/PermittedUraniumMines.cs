using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PermittedUraniumMines
    {
        public int Xid { get; set; }
        public string Mineid { get; set; }
        public string County { get; set; }
        public string Type { get; set; }
        public string Num { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string Operator { get; set; }
        public string Product { get; set; }
        public string MinType { get; set; }
        public string PermStat { get; set; }
        public string Msu { get; set; }
        public string SurfOwn { get; set; }
        public string MinOwn { get; set; }
        public string Contact { get; set; }
        public decimal? OperId { get; set; }
        public decimal? AcrRel { get; set; }
        public decimal? Easting { get; set; }
        public decimal? Northing { get; set; }
        public string Meridian { get; set; }
        public string Township { get; set; }
        public string Range { get; set; }
        public string Section { get; set; }
        public Point Shape { get; set; }
    }
}
