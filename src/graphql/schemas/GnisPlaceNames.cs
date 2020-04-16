using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace graphql.schemas
{
    public partial class GnisPlaceNames
    {
        public int Xid { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string County { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Long { get; set; }
        public decimal? Elevation { get; set; }
        public decimal? ElevationMeters { get; set; }
        public string QuadName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EditedDate { get; set; }
        public Point Shape { get; set; }
    }
}
