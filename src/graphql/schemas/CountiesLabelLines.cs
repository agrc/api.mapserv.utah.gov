using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class CountiesLabelLines
    {
        public int Xid { get; set; }
        public string Rightcounty { get; set; }
        public string Leftcounty { get; set; }
        public MultiLineString Shape { get; set; }
    }
}
