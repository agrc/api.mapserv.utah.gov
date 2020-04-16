using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaBusFlexRoutes
    {
        public int Xid { get; set; }
        public string Lineabbr { get; set; }
        public string Linename { get; set; }
        public string Frequency { get; set; }
        public string Routetype { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
