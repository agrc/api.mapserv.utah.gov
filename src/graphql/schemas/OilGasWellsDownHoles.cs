using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class OilGasWellsDownHoles
    {
        public int Xid { get; set; }
        public decimal? Locid { get; set; }
        public string Api { get; set; }
        public decimal? Constructnumber { get; set; }
        public string Constructtype { get; set; }
        public string Constructstatus { get; set; }
        public string Loctype { get; set; }
        public string Elevationtype { get; set; }
        public decimal? Depth { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? CoordsbhN { get; set; }
        public decimal? CoordsbhE { get; set; }
        public string Utmzone { get; set; }
        public decimal? Footagens { get; set; }
        public string DirNs { get; set; }
        public decimal? Footageew { get; set; }
        public string DirEw { get; set; }
        public string Qtrqtr { get; set; }
        public decimal? Section { get; set; }
        public decimal? Township { get; set; }
        public string Townshipdir { get; set; }
        public decimal? Range { get; set; }
        public string Rangedir { get; set; }
        public string Meridian { get; set; }
        public decimal? County { get; set; }
        public Point Shape { get; set; }
    }
}
