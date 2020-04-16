using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class UtaBusRoutesWasatchFront
    {
        public int Xid { get; set; }
        public string Lineabbr { get; set; }
        public string Linename { get; set; }
        public string Frequency { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public decimal? Avgbrd { get; set; }
        public decimal? ShapeLen { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
