using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StateFacilityLocations
    {
        public int Xid { get; set; }
        public string Status { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
        public decimal? Build { get; set; }
        public string Buildname { get; set; }
        public decimal? Agencyid { get; set; }
        public string Type { get; set; }
        public string Organizati { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Countyabre { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public decimal? Constructd { get; set; }
        public decimal? Squareft { get; set; }
        public string Classabrev { get; set; }
        public decimal? Usevalue { get; set; }
        public decimal? Cvalue { get; set; }
        public decimal? Valratenz { get; set; }
        public string Their1 { get; set; }
        public decimal? RiskNumbe { get; set; }
        public string CorrectionBldNr { get; set; }
        public decimal? RiskManBldNr { get; set; }
        public Point Shape { get; set; }
    }
}
