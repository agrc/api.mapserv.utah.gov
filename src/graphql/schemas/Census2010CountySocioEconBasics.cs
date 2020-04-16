using System;
using System.Collections.Generic;

namespace graphql.schemas
{
    public partial class Census2010CountySocioEconBasics
    {
        public int Xid { get; set; }
        public string Fips { get; set; }
        public string Name { get; set; }
        public decimal? Landsqmile { get; set; }
        public decimal? Pop10 { get; set; }
        public decimal? Houseunits { get; set; }
        public decimal? Racewhite { get; set; }
        public decimal? Raceblack { get; set; }
        public decimal? Racehispanic { get; set; }
        public decimal? Raceasian { get; set; }
        public decimal? Racenatam { get; set; }
        public decimal? Incltpoverty { get; set; }
        public decimal? Medianinc { get; set; }
        public decimal? Inclt25k { get; set; }
        public decimal? Inc25to50k { get; set; }
        public decimal? Inc50to100k { get; set; }
        public decimal? Inc100to200k { get; set; }
        public decimal? Incgt200k { get; set; }
        public decimal? Hsgrad { get; set; }
        public decimal? Bachgrad { get; set; }
        public decimal? Agelt5 { get; set; }
        public decimal? Age5to9 { get; set; }
        public decimal? Age20to34 { get; set; }
        public decimal? Age35to59 { get; set; }
        public decimal? Agegt60 { get; set; }
    }
}
