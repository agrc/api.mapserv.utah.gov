using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CountiesModifications
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public string Name { get; set; }
        public decimal? Fips { get; set; }
        public string Stateplane { get; set; }
        public DateTime? Updated { get; set; }
        public string UpdateType { get; set; }
        public string DocLoc { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
