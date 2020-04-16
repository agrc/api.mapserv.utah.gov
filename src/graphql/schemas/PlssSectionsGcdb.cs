using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PlssSectionsGcdb
    {
        public int Xid { get; set; }
        public string Frstdivid { get; set; }
        public string Township { get; set; }
        public string Range { get; set; }
        public string Section { get; set; }
        public string Basemeridian { get; set; }
        public string Label { get; set; }
        public string Frstdivno { get; set; }
        public string Frstdivdup { get; set; }
        public string Frstdivtyp { get; set; }
        public string Frstdivtxt { get; set; }
        public string Plssid { get; set; }
        public string Frstdivlab { get; set; }
        public string Survtyp { get; set; }
        public string Survtyptxt { get; set; }
        public decimal? Tnum { get; set; }
        public decimal? Rnum { get; set; }
        public decimal? Snum { get; set; }
        public decimal? Qnum { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
