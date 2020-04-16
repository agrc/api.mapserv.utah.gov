using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class StreamsNhd
    {
        public int Xid { get; set; }
        public string PermanentIdentifier { get; set; }
        public DateTime? Fdate { get; set; }
        public decimal? Resolution { get; set; }
        public string GnisId { get; set; }
        public string GnisName { get; set; }
        public decimal? Lengthkm { get; set; }
        public string Reachcode { get; set; }
        public decimal? Flowdir { get; set; }
        public string WbareaPermanentIdentifier { get; set; }
        public decimal? Ftype { get; set; }
        public string FtypeText { get; set; }
        public decimal? Fcode { get; set; }
        public string FcodeText { get; set; }
        public decimal? Enabled { get; set; }
        public decimal? Inutah { get; set; }
        public decimal? Ismajor { get; set; }
        public decimal? Submerged { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
