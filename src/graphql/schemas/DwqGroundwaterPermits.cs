using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqGroundwaterPermits
    {
        public int Xid { get; set; }
        public decimal? Rec { get; set; }
        public string Permittee { get; set; }
        public string Permitted { get; set; }
        public string PermitNo { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? Expiration { get; set; }
        public string Manager { get; set; }
        public string Sector { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Wmu { get; set; }
        public decimal? Nad83utmX { get; set; }
        public decimal? Nad83utmY { get; set; }
        public string LocAddres { get; set; }
        public string City { get; set; }
        public string StateZip { get; set; }
        public string Maplabel { get; set; }
        public string WebInfo { get; set; }
        public Point Shape { get; set; }
    }
}
