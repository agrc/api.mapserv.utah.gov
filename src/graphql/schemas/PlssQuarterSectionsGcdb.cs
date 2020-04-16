using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class PlssQuarterSectionsGcdb
    {
        public int Xid { get; set; }
        public string Plssid { get; set; }
        public string Refgridno { get; set; }
        public string Refgridnom { get; set; }
        public string Refgriddup { get; set; }
        public string Qsection { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
