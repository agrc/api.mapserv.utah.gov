using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DeqEnforceableWrittenAssurances
    {
        public int Xid { get; set; }
        public decimal? StKey { get; set; }
        public string Derrid { get; set; }
        public string Cimid { get; set; }
        public string Maplabel { get; set; }
        public string Sitedesc { get; set; }
        public string Sitename { get; set; }
        public string Siteaddres { get; set; }
        public string Sitecity { get; set; }
        public string Sitecnty { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Zip4 { get; set; }
        public string Currprojma { get; set; }
        public string Conphone { get; set; }
        public decimal? Northing { get; set; }
        public decimal? Easting { get; set; }
        public string Projdesc { get; set; }
        public Point Shape { get; set; }
    }
}
