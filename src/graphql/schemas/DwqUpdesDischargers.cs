using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqUpdesDischargers
    {
        public int Xid { get; set; }
        public decimal? Rec { get; set; }
        public string NpdesId { get; set; }
        public string Permittee { get; set; }
        public string Permittype { get; set; }
        public string Permitname { get; set; }
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string LocAddres { get; set; }
        public string SupAddres { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string StateZip { get; set; }
        public string County { get; set; }
        public string Descript { get; set; }
        public string RecWaters { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? EffecDate { get; set; }
        public DateTime? ExpirDate { get; set; }
        public string SicCode { get; set; }
        public decimal? LatDd { get; set; }
        public decimal? LongDd { get; set; }
        public string PermitGrp { get; set; }
        public decimal? Nad83utmY { get; set; }
        public decimal? Nad83utmX { get; set; }
        public string Maplabel { get; set; }
        public decimal? PointX { get; set; }
        public decimal? PointY { get; set; }
        public Point Shape { get; set; }
    }
}
