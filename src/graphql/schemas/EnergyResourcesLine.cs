using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class EnergyResourcesLine
    {
        public int Xid { get; set; }
        public string Code { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
