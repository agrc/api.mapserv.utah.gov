using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AddressPoints
    {
        public int Xid { get; set; }
        public string Addsystem { get; set; }
        public string Utaddptid { get; set; }
        public string Fulladd { get; set; }
        public string Addnum { get; set; }
        public string Addnumsuffix { get; set; }
        public string Prefixdir { get; set; }
        public string Streetname { get; set; }
        public string Streettype { get; set; }
        public string Suffixdir { get; set; }
        public string Landmarkname { get; set; }
        public string Building { get; set; }
        public string Unittype { get; set; }
        public string Unitid { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public string Countyid { get; set; }
        public string State { get; set; }
        public string Ptlocation { get; set; }
        public string Pttype { get; set; }
        public string Structure { get; set; }
        public string Parcelid { get; set; }
        public string Addsource { get; set; }
        public DateTime? Loaddate { get; set; }
        public string Usng { get; set; }
        public Point Shape { get; set; }
    }
}
