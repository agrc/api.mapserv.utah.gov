using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class VistaBallotAreasProposed
    {
        public int Xid { get; set; }
        public decimal? Countyid { get; set; }
        public string Vistaid { get; set; }
        public string Precinctid { get; set; }
        public string Subprecinctid { get; set; }
        public string Versionnbr { get; set; }
        public DateTime? Effectivedate { get; set; }
        public string Aliasname { get; set; }
        public string Comments { get; set; }
        public DateTime? Rcvddate { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
