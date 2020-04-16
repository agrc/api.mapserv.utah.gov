using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class TaxEntities2018
    {
        public int Xid { get; set; }
        public string EntId { get; set; }
        public decimal? EntYr { get; set; }
        public string EntCo { get; set; }
        public decimal? EntNbr { get; set; }
        public string EntDesc { get; set; }
        public decimal? Temp1 { get; set; }
        public decimal? Temp2 { get; set; }
        public string EntCode { get; set; }
        public decimal? EntRate { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
