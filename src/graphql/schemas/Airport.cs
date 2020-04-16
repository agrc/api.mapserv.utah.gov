using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Airport
    {
        public int Xid { get; set; }
        public string SiteNo { get; set; }
        public string FacType { get; set; }
        public string LocId { get; set; }
        public string CountyNam { get; set; }
        public string CityName { get; set; }
        public string Fullname { get; set; }
        public string OwnerType { get; set; }
        public decimal? Elev { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
