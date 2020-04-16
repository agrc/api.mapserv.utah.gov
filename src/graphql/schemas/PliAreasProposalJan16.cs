using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PliAreasProposalJan16
    {
        public int Xid { get; set; }
        public string NameProposed { get; set; }
        public string DesignationProposed { get; set; }
        public string County { get; set; }
        public decimal? Acreage { get; set; }
        public string Class { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
