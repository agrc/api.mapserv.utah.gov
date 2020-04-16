using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CoalDepositAreas1988
    {
        public int Xid { get; set; }
        public decimal? Acreage { get; set; }
        public string KmdaName { get; set; }
        public decimal? KmdaNumbe { get; set; }
        public decimal? FavCode { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
