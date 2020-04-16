using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EarthquakeEpicenters1850To2016
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
        public string DepthQual { get; set; }
        public string DepthChge { get; set; }
        public string EqFlag { get; set; }
        public string SourceEpi { get; set; }
        public string MagUu { get; set; }
        public string MaguuFlag { get; set; }
        public string MlUu { get; set; }
        public string McUu { get; set; }
        public string Nph { get; set; }
        public string Gap { get; set; }
        public string Dmin { get; set; }
        public string Rms { get; set; }
        public string Erh { get; set; }
        public string Erz { get; set; }
        public Point Shape { get; set; }
    }
}
