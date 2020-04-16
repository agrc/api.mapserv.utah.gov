using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CensusPlacePoints2010
    {
        public int Xid { get; set; }
        public decimal? Geographyid { get; set; }
        public string Geographyname { get; set; }
        public decimal? Landarea { get; set; }
        public decimal? Population { get; set; }
        public decimal? Households { get; set; }
        public decimal? Medianincome { get; set; }
        public decimal? Incomebelowpoverty { get; set; }
        public decimal? Incomelessthan25 { get; set; }
        public decimal? Incomebetween25to50 { get; set; }
        public decimal? Incomebetween50to100 { get; set; }
        public decimal? Incomebetween100to200 { get; set; }
        public decimal? Incomegreater200 { get; set; }
        public decimal? Educationhighschoolgraduate { get; set; }
        public decimal? Educationbachelororgreater { get; set; }
        public decimal? Ageunder5 { get; set; }
        public decimal? Agebetween5to19 { get; set; }
        public decimal? Agebetween20to34 { get; set; }
        public decimal? Agebetween35to59 { get; set; }
        public decimal? Agegreaterthan60 { get; set; }
        public decimal? Raceasian { get; set; }
        public decimal? Raceblack { get; set; }
        public decimal? Racehispanic { get; set; }
        public decimal? Racenativeamerican { get; set; }
        public decimal? Racewhite { get; set; }
        public decimal? Censusyear { get; set; }
        public decimal? UsdaRuralityscore { get; set; }
        public decimal? UsdaEconomicscore { get; set; }
        public decimal? UsdaCombinedneed { get; set; }
        public decimal? Pctutahmedianincome { get; set; }
        public Point Shape { get; set; }
    }
}
