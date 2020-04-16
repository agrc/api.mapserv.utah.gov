using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class DwqMercuryInFishTissue
    {
        public int Xid { get; set; }
        public string MonlocId { get; set; }
        public string Species { get; set; }
        public decimal? Year { get; set; }
        public string SampleGrp { get; set; }
        public string SiteDescr { get; set; }
        public decimal? Average { get; set; }
        public decimal? SampleNum { get; set; }
        public decimal? StDev { get; set; }
        public decimal? MinHg { get; set; }
        public decimal? MaxHg { get; set; }
        public string County { get; set; }
        public string FishAdvis { get; set; }
        public string Water { get; set; }
        public decimal? Latnad83dd { get; set; }
        public decimal? Lonnad83dd { get; set; }
        public string Maplabel { get; set; }
        public decimal? Nad83utmX { get; set; }
        public decimal? Nad83utmY { get; set; }
        public decimal? AveLength { get; set; }
        public decimal? AveWeight { get; set; }
        public Point Shape { get; set; }
    }
}
