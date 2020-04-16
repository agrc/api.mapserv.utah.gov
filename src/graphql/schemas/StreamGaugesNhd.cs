using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StreamGaugesNhd
    {
        public int Xid { get; set; }
        public decimal? Comid { get; set; }
        public DateTime? Eventdate { get; set; }
        public string Reachcode { get; set; }
        public DateTime? Reachsmdate { get; set; }
        public decimal? Reachresolution { get; set; }
        public decimal? Featurecomid { get; set; }
        public decimal? Featureclassref { get; set; }
        public string SourceOriginator { get; set; }
        public string SourceDatadesc { get; set; }
        public string SourceFeatureid { get; set; }
        public string Featuredetailurl { get; set; }
        public decimal? Offset { get; set; }
        public decimal? Measure { get; set; }
        public decimal? Eventtype { get; set; }
        public string Guagename { get; set; }
        public string Guageid { get; set; }
        public decimal? StreamFlow { get; set; }
        public decimal? GaugeHeight { get; set; }
        public DateTime? LastUpdate { get; set; }
        public decimal? HighestForecast { get; set; }
        public DateTime? HighestForecastDate { get; set; }
        public decimal? LastForecast { get; set; }
        public DateTime? LastForecastDate { get; set; }
        public decimal? Flood { get; set; }
        public decimal? Moderate { get; set; }
        public decimal? Major { get; set; }
        public decimal? Actionstage { get; set; }
        public string Currentstage { get; set; }
        public DateTime? Lastactionorabove { get; set; }
        public Point Shape { get; set; }
    }
}
