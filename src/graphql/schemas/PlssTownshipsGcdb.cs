using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PlssTownshipsGcdb
    {
        public int Xid { get; set; }
        public string Plssid { get; set; }
        public string Township { get; set; }
        public string Range { get; set; }
        public string Basemeridian { get; set; }
        public string Label { get; set; }
        public string Stateabbr { get; set; }
        public string Prinmercd { get; set; }
        public string Prinmer { get; set; }
        public string Twnshpno { get; set; }
        public string Twnshpfrac { get; set; }
        public string Twnshpdir { get; set; }
        public string Rangeno { get; set; }
        public string Rangefrac { get; set; }
        public string Rangedir { get; set; }
        public string Twnshpdpcd { get; set; }
        public string Steward { get; set; }
        public string Twnshplab { get; set; }
        public string Srvname { get; set; }
        public string Secsrvname { get; set; }
        public string Survtyp { get; set; }
        public string Survtyptxt { get; set; }
        public decimal? Tnum { get; set; }
        public decimal? Rnum { get; set; }
        public decimal? Qnum { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
