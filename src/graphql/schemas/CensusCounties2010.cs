using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CensusCounties2010
    {
        public int Xid { get; set; }
        public string Statefp10 { get; set; }
        public string Countyfp10 { get; set; }
        public string Countyns10 { get; set; }
        public string Geoid10 { get; set; }
        public string Name10 { get; set; }
        public string Namelsad10 { get; set; }
        public string Lsad10 { get; set; }
        public string Classfp10 { get; set; }
        public string Mtfcc10 { get; set; }
        public string Csafp10 { get; set; }
        public string Cbsafp10 { get; set; }
        public string Metdivfp10 { get; set; }
        public string Funcstat10 { get; set; }
        public decimal? Aland10 { get; set; }
        public decimal? Awater10 { get; set; }
        public string Intptlat10 { get; set; }
        public string Intptlon10 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
