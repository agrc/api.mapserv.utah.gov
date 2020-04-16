using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class OilGasWellsPaths
    {
        public int Xid { get; set; }
        public string Api { get; set; }
        public decimal? Constructnumber { get; set; }
        public string Constructstatus { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
