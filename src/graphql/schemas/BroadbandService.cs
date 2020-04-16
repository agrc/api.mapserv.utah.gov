using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class BroadbandService
    {
        public int Xid { get; set; }
        public string Utprovcode { get; set; }
        public decimal? Transtech { get; set; }
        public decimal? Maxaddown { get; set; }
        public decimal? Maxadup { get; set; }
        public DateTime? Lastedit { get; set; }
        public DateTime? Lastverified { get; set; }
        public string Identifier { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
