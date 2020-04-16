using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class FirePerimeters
    {
        public int Xid { get; set; }
        public string Firename { get; set; }
        public string Firecode { get; set; }
        public string Incidentnumber { get; set; }
        public string Agency { get; set; }
        public decimal? Perimeteryear { get; set; }
        public DateTime? Perimeterdate { get; set; }
        public string Perimetertime { get; set; }
        public string Source { get; set; }
        public decimal? Blm { get; set; }
        public decimal? Br { get; set; }
        public decimal? Dnr { get; set; }
        public decimal? Dod { get; set; }
        public decimal? Doe { get; set; }
        public decimal? Nps { get; set; }
        public decimal? Os { get; set; }
        public decimal? Private { get; set; }
        public decimal? Sitla { get; set; }
        public decimal? SlF { get; set; }
        public decimal? Tribal { get; set; }
        public decimal? Udot { get; set; }
        public decimal? Udwr { get; set; }
        public decimal? Usfs { get; set; }
        public decimal? Usfws { get; set; }
        public decimal? Usp { get; set; }
        public decimal? Ffsl { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
