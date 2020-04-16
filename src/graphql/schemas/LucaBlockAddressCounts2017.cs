using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class LucaBlockAddressCounts2017
    {
        public int Xid { get; set; }
        public string Geoid10 { get; set; }
        public string Ur10 { get; set; }
        public string Uace10 { get; set; }
        public string Uatype { get; set; }
        public decimal? ResaddCt { get; set; }
        public decimal? GcaddCt { get; set; }
        public decimal? UtaddCt { get; set; }
        public decimal? UtCbDiff { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
