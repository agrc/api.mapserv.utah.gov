using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class Trailheads
    {
        public int Xid { get; set; }
        public string Primaryname { get; set; }
        public string Trailheadid { get; set; }
        public string Features { get; set; }
        public string Primarymaintenance { get; set; }
        public string Seasonalrestriction { get; set; }
        public string Infourl { get; set; }
        public string Comments { get; set; }
        public string Datasource { get; set; }
        public DateTime? Lastupdate { get; set; }
        public Point Shape { get; set; }
    }
}
