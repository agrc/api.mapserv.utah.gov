using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UndergroundInjectionControlFacility
    {
        public int Xid { get; set; }
        public string Guid { get; set; }
        public string Frsid { get; set; }
        public decimal? Countyfips { get; set; }
        public string Naicsprimary { get; set; }
        public string Naicssecondary { get; set; }
        public string Facilityid { get; set; }
        public string Facilitytype { get; set; }
        public string Facilityname { get; set; }
        public string Facilityaddress { get; set; }
        public string Facilitycity { get; set; }
        public string Facilitystate { get; set; }
        public string Facilityzip { get; set; }
        public string Facilitymilepost { get; set; }
        public string Nomigrationpetstatus { get; set; }
        public string Comments { get; set; }
        public DateTime? Createdon { get; set; }
        public DateTime? Modifiedon { get; set; }
        public string Editedby { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
