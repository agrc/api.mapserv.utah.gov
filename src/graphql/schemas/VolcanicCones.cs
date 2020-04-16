using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class VolcanicCones
    {
        public int Xid { get; set; }
        public string SType { get; set; }
        public Point Shape { get; set; }
    }
}
