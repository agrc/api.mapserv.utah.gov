using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GeothermalLeases
    {
        public int Xid { get; set; }
        public decimal? FidGbcada { get; set; }
        public decimal? Township { get; set; }
        public decimal? Range { get; set; }
        public string D { get; set; }
        public string E { get; set; }
        public string Meridian { get; set; }
        public decimal? Section { get; set; }
        public string SerialNr { get; set; }
        public string AdminStat { get; set; }
        public string Mtrs { get; set; }
        public decimal? Mblm { get; set; }
        public decimal? Tblm { get; set; }
        public string Dblm { get; set; }
        public decimal? Rblm { get; set; }
        public string Eblm { get; set; }
        public decimal? Sblm { get; set; }
        public string CustNm { get; set; }
        public string SerialNr1 { get; set; }
        public string AdminSt1 { get; set; }
        public string Mtrs1 { get; set; }
        public decimal? Mblm1 { get; set; }
        public decimal? Tblm1 { get; set; }
        public string Dblm1 { get; set; }
        public decimal? Rblm1 { get; set; }
        public string Eblm1 { get; set; }
        public decimal? Sblm1 { get; set; }
        public string CustNm1 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
