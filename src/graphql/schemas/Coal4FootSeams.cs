using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Coal4FootSeams
    {
        public int Xid { get; set; }
        public decimal? Id { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
