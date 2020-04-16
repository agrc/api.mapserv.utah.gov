using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GsenmSpecialRecreationManagementArea
    {
        public int Xid { get; set; }
        public string SrmaId { get; set; }
        public string SrmaName { get; set; }
        public decimal? Acres { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
