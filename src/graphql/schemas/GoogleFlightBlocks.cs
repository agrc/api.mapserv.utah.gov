using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GoogleFlightBlocks
    {
        public int Xid { get; set; }
        public string Resolution { get; set; }
        public string Tile { get; set; }
        public string Path { get; set; }
        public string Ext { get; set; }
        public decimal? Size { get; set; }
        public DateTime? Date { get; set; }
        public string Worldfile { get; set; }
        public string Metadata { get; set; }
        public decimal? Sqmiles { get; set; }
        public decimal? Sqkm { get; set; }
        public decimal? Year { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
