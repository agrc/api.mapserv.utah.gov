using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class RoadsFreewayExits
    {
        public int Xid { get; set; }
        public string Exitname { get; set; }
        public string Exitnbr { get; set; }
        public string Rtname { get; set; }
        public string DotRtname { get; set; }
        public Point Shape { get; set; }
    }
}
