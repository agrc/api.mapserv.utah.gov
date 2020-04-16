using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PublicSafetyAnsweringPointBoundaries
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public string PsapJuris { get; set; }
        public string PsapName { get; set; }
        public string County { get; set; }
        public string Fips { get; set; }
        public string Ecatsid { get; set; }
        public decimal? Color4 { get; set; }
        public string PhoneNumber { get; set; }
        public string DpsPsapName { get; set; }
        public string Text911 { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
