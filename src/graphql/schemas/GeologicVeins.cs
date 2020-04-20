﻿using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GeologicVeins
    {
        public int Xid { get; set; }
        public string LType { get; set; }
        public string Modifier { get; set; }
        public string Accuracy { get; set; }
        public string FaultCon { get; set; }
        public decimal? Source { get; set; }
        public string LineDescr { get; set; }
        public MultiLineString Shape { get; set; }
    }
}