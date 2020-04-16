using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class MajorLakes
    {
        public int Xid { get; set; }
        public decimal? Wb { get; set; }
        public decimal? WbId { get; set; }
        public decimal? ComId { get; set; }
        public decimal? RchComId { get; set; }
        public string Ftype { get; set; }
        public decimal? Fcode { get; set; }
        public decimal? Elev { get; set; }
        public string Stage { get; set; }
        public decimal? SqKm { get; set; }
        public string GnisId { get; set; }
        public string Name { get; set; }
        public MultiPolygon Shape { get; set; }
    }
}
