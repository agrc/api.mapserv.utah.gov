using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class ZipCodeAreas
    {
        public int Xid { get; set; }
        public string Zip5 { get; set; }
        public string Countynbr { get; set; }
        public string Name { get; set; }
        public decimal? Symbol { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
