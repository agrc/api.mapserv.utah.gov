using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DeqToxicReleaseInventory
    {
        public int Xid { get; set; }
        public decimal? StKey { get; set; }
        public string Derrid { get; set; }
        public string Cimid { get; set; }
        public string Maplabel { get; set; }
        public string Sitedesc { get; set; }
        public string FacName { get; set; }
        public string FacAddres { get; set; }
        public string FacCity { get; set; }
        public string FacCnty { get; set; }
        public string FacState { get; set; }
        public string FacZip { get; set; }
        public decimal? Easting { get; set; }
        public decimal? Northing { get; set; }
        public Point Shape { get; set; }
    }
}
