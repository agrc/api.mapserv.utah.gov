using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UpdesSites
    {
        public int Xid { get; set; }
        public string NpdesId { get; set; }
        public string Facility { get; set; }
        public string Permitee { get; set; }
        public string Official { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Location { get; set; }
        public string SuppAddr { get; set; }
        public string SuppCity { get; set; }
        public string SupState { get; set; }
        public string SupZip { get; set; }
        public string County { get; set; }
        public string Waterbody { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? EffecDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public string PrimCode { get; set; }
        public string LatDeg { get; set; }
        public string LongDeg { get; set; }
        public decimal? Nad83utmX { get; set; }
        public decimal? Nad83utmY { get; set; }
        public string Study { get; set; }
        public Point Shape { get; set; }
    }
}
