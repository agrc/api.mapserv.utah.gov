using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CensusBlockGroups2010
    {
        public int Xid { get; set; }
        public string Statefp10 { get; set; }
        public string Countyfp10 { get; set; }
        public string Tractce10 { get; set; }
        public string Blockce10 { get; set; }
        public string Geoid10 { get; set; }
        public string Name10 { get; set; }
        public string Funcstat10 { get; set; }
        public decimal? Aland10 { get; set; }
        public decimal? Awater10 { get; set; }
        public string Intptlat10 { get; set; }
        public string Intptlon10 { get; set; }
        public string Logrecno { get; set; }
        public decimal? Pop100 { get; set; }
        public decimal? Hu100 { get; set; }
        public decimal? P0020001 { get; set; }
        public decimal? P0020002 { get; set; }
        public decimal? P0020003 { get; set; }
        public decimal? P0020004 { get; set; }
        public decimal? P0020005 { get; set; }
        public decimal? P0020006 { get; set; }
        public decimal? P0020007 { get; set; }
        public decimal? P0020008 { get; set; }
        public decimal? P0020009 { get; set; }
        public decimal? P0020010 { get; set; }
        public string Mtfcc { get; set; }
        public decimal? P0010011 { get; set; }
        public decimal? P0010012 { get; set; }
        public decimal? P0010013 { get; set; }
        public decimal? P0010014 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
