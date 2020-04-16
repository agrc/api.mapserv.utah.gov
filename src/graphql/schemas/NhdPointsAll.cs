using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class NhdPointsAll
    {
        public int Xid { get; set; }
        public string PermanentIdentifier { get; set; }
        public DateTime? Fdate { get; set; }
        public decimal? Resolution { get; set; }
        public string GnisId { get; set; }
        public string GnisName { get; set; }
        public string Reachcode { get; set; }
        public decimal? Ftype { get; set; }
        public string FtypeText { get; set; }
        public decimal? Fcode { get; set; }
        public string FcodeText { get; set; }
        public decimal? Inutah { get; set; }
        public Point Shape { get; set; }
    }
}
