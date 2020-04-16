using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TransmissionLines
    {
        public int Xid { get; set; }
        public string SgidDesU { get; set; }
        public string Layer { get; set; }
        public decimal? Elevation { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? Color { get; set; }
        public string Comments { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
