using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PlssQuarterQuarterSectionsGcdb
    {
        public int Xid { get; set; }
        public string Secdivid { get; set; }
        public string Frstdivid { get; set; }
        public string Secdivno { get; set; }
        public string Secdivsuf { get; set; }
        public string Secdivtyp { get; set; }
        public string Secdivtxt { get; set; }
        public decimal? Acres { get; set; }
        public string Plssid { get; set; }
        public string Secdivlab { get; set; }
        public string Survtyp { get; set; }
        public string Survtyptxt { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
