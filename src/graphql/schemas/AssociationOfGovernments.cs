using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class AssociationOfGovernments
    {
        public int Xid { get; set; }
        public string Countynbr { get; set; }
        public string Name { get; set; }
        public decimal? Fips { get; set; }
        public string AogName { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
