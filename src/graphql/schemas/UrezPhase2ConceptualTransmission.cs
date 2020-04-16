using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UrezPhase2ConceptualTransmission
    {
        public int Xid { get; set; }
        public string Datasource { get; set; }
        public string Subid1 { get; set; }
        public string Subid2 { get; set; }
        public decimal? Miles { get; set; }
        public string Label { get; set; }
        public string Status { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
