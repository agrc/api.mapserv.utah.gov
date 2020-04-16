﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class HexagonGrid1km
    {
        public int Xid { get; set; }
        public decimal? Hexid { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
