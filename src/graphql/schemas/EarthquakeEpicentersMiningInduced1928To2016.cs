using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EarthquakeEpicentersMiningInduced1928To2016
    {
        public int Xid { get; set; }
        public decimal? Mag { get; set; }
        public decimal? Long { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Year { get; set; }
        public decimal? Mo { get; set; }
        public decimal? Day { get; set; }
        public decimal? Hr { get; set; }
        public decimal? Min { get; set; }
        public decimal? Sec { get; set; }
        public decimal? Sigm { get; set; }
        public decimal? Round { get; set; }
        public string MagType { get; set; }
        public string EpiQual { get; set; }
        public decimal? DepthQual { get; set; }
        public string DepthChge { get; set; }
        public string EqFlag { get; set; }
        public string SourceEpi { get; set; }
        public decimal? MagUu { get; set; }
        public string MaguuFlag { get; set; }
        public string MlUu { get; set; }
        public decimal? McUu { get; set; }
        public decimal? Nph { get; set; }
        public decimal? Gap { get; set; }
        public decimal? Dmin { get; set; }
        public decimal? Rms { get; set; }
        public decimal? Erh { get; set; }
        public decimal? Erz { get; set; }
        public Point Shape { get; set; }
    }
}
