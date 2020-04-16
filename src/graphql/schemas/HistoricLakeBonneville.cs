using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HistoricLakeBonneville
    {
        public int Xid { get; set; }
        public int? Objectid { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
